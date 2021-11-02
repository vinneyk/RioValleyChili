using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.DataSeeders.Utilities
{
    public class PickedInventoryKeyHelper
    {
        public static PickedInventoryKeyHelper Singleton
        {
            get
            {
                if(_singleton == null)
                {
                    using(var oldContext = ContextsHelper.GetOldContext())
                    {
                        _singleton = new PickedInventoryKeyHelper(oldContext);
                    }
                }
                return _singleton;
            }
        }

        private static PickedInventoryKeyHelper _singleton;

        private PickedInventoryKeyHelper(RioAccessSQLEntities oldContext)
        {
            var customerOrderKey = new SalesOrderKey();
            foreach(var reservedKey in oldContext.tblOrders.Where(o => o.SerializedKey != null).Select(o => o.SerializedKey))
            {
                ISalesOrderKey parsedKey;
                if(customerOrderKey.TryParse(reservedKey, out parsedKey))
                {
                    GetRange(parsedKey.SalesOrderKey_DateCreated).RegisterSequence(parsedKey.SalesOrderKey_Sequence);
                }
            }
        }

        public int GetNextSequence(DateTime d)
        {
            return GetRange(d).GetNextSequence();
        }

        private Range GetRange(DateTime d)
        {
            d = d.Date;
            Range range;
            if(!_sequences.TryGetValue(d, out range))
            {
                _sequences.Add(d, range = new Range());
            }
            return range;
        }
        private readonly Dictionary<DateTime, Range> _sequences = new Dictionary<DateTime, Range>();

        private class Range
        {
            public int GetNextSequence()
            {
                AdvanceLast();
                return _last;
            }

            private int _first = -1;
            private int _last = -1;
            private Range _next;

            public void RegisterSequence(int sequence)
            {
                if(sequence >= _first && sequence <= _last)
                {
                    return;
                }

                if(sequence < _first)
                {
                    _next = new Range
                        {
                            _first = _first,
                            _last = _last,
                            _next = _next
                        };
                    _first = _last = sequence;
                }
                else if(sequence > _last)
                {
                    if(sequence == _last + 1)
                    {
                        AdvanceLast();
                    }
                    else
                    {
                        if(_next != null)
                        {
                            _next.RegisterSequence(sequence);
                        }
                        else
                        {
                            _next = new Range
                                {
                                    _first = sequence,
                                    _last = sequence
                                };
                        }
                    }
                }
            }

            private void AdvanceLast()
            {
                _last++;
                if(_next != null && _last >= _next._first)
                {
                    _last = _next._last;
                    _next = _next._next;
                    AdvanceLast();
                }
            }
        }
    }
}