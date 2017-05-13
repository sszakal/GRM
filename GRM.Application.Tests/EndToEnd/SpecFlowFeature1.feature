Feature: Query Music Contracts

Scenario: 1
Given the supplied above reference data
When user enters 'ITunes 1st March 2012'
Then the output is: Monkey Claw|Black Mountain|digital download|1st Feb 2012|
And the output is: Monkey Claw|Motor Mouth|digital download|1st Mar 2011|
And the output is: Tinie Tempah|Frisky (Live from SoHo)|digital download|1st Feb 2012|
And the output is: Tinie Tempah|Miami 2 Ibiza|digital download|1st Feb 2012|

Scenario: 2
Given the supplied above reference data
When user enters 'YouTube 1st April 2012'
Then the output is: Monkey Claw|Motor Mouth|streaming|1st Mar 2011|
And the output is: Tinie Tempah|Frisky (Live from SoHo)|streaming|1st Feb 2012|

Scenario: 3
Given the supplied above reference data
When user enters 'YouTube 27st Dec 2012'
Then the output is: Monkey Claw|Christmas Special|streaming|25st Dec 2012|31st Dec 2012
And the output is: Monkey Claw|Iron Horse|streaming|1st Jun 2012|
And the output is: Monkey Claw|Motor Mouth|streaming|1st Mar 2011|
And the output is: Tinie Tempah|Frisky (Live from SoHo)|streaming|1st Feb 2012|
