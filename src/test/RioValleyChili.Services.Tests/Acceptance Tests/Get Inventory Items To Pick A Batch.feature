Feature: Get Inventory Items To Pick A Batch

Scenario: Inventory in an invalid quality state is not available for picking
	Given inventory from a lot in any of the following states
	| Quality State      | Hold | Location |	
	| Released           | hold | unlocked |
	| Released           | none | locked   |
	When I get inventory to pick for a batch
	Then the inventory items will not be included in the results