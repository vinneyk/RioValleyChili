Feature: Calculate Attribute Weighted Averages

Scenario: Inventory attribute values are weighted in as expected
	Given inventory with the following attributes
	| Quantity | Packaging Weight | Asta | Scan | Scov |
	| 4        | 100              | 80   | 50   | 60   |
	| 1        | 200              |      | 75   | 44   |
	| 5        | 25               | 100  |      | 70   |
	When I calculate attribute weighted averages
	Then the attribute weighted average results are as expected
	| Asta	 | Scan	   | Scoville	|
	| 61.38  | 48.28   | 57.31      |