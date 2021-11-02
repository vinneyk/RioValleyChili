Feature: Set Picked Inventory For Production Batch	

Scenario: Inventory in an invalid quality state cannot be picked
	Given a production batch that has not been produced
	And inventory from a lot in any of the following states
	| Quality State      | Hold | Location |
	| Pending            | none | unlocked |	
	| Contaminated       | none | unlocked |
	| Rejected           | none | unlocked |
	| Released           | hold | unlocked |
	| Released           | none | locked   |
	When I attempt to pick any of the above inventory for the batch
	Then the service method returns an invalid result
	
Scenario: Production Batch that has been produced cannot have its Picked Inventory modified
	Given a production batch that has been produced
	When I set picked inventory for the production batch
	Then the service method returns an invalid result

Scenario: Picking inventory for a production batch will set resulting lot attributes to the weighted average values of the items picked
	Given a production batch that has not been produced
	And inventory with the following attributes         
	| Quantity | Packaging Weight | AIA | Ash | AToxin |
	| 4        | 100              | 80  | 50  | 60     |
	| 1        | 200              |     | 75  | 44     |
	| 5        | 25               | 100 |     | 70     |
	When I pick the above inventory for the batch
	Then the batch's resulting lot will have its attribute values set to
	| AIA   | Ash   | AToxin |
	| 61.38 | 48.28 | 57.31  |