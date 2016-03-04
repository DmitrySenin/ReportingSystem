var should = require('chai').should();
var TimeSpan = require('timespan');

var StampType = require('../reports/StampType.js');
var EmployerTimeStamp = require('../reports/EmployerTimeStamp.js');
var TimeOfWorkReport = require('../reports/TimeOfWorkForDayReport.js');

var FakeStampsSource = require('./FakeStampsSource.js');

describe("TimeOfWorkReport.CreateReport", function() {

	it("Should correct compute time of work since data is correct", function() {

		var targetEmployerID = 1;

		// 03.04.2016
		var targetDay = new Date(2016, 3, 3);

		stamps = [
			// In-stamp at 9.00 am
			new EmployerTimeStamp( targetEmployerID, StampType.In, new Date(targetDay.setHours(9)) ),

			// Out-stamp at 10.00 am
			new EmployerTimeStamp( targetEmployerID, StampType.Out, new Date(targetDay.setHours(10)) ),

			// In-stamp at 12.00 pm
			new EmployerTimeStamp( targetEmployerID, StampType.In, new Date(targetDay.setHours(12)) ),

			// Out-stamp at 6.00 pm
			new EmployerTimeStamp( targetEmployerID, StampType.Out, new Date(targetDay.setHours(18)) ),
		];

		// Create fake source.
		var source = new FakeStampsSource.FakeStampsSource(stamps);
		var reporter = new TimeOfWorkReport(source);

		// Expected time of work should be 7 hours (because (18 - 12) + (10 - 9) = 7 ).
		var expectedTimeOfWork = TimeSpan.fromHours(7);

		var protocol = reporter.CreateReport(targetEmployerID, targetDay); 

		protocol.IsSucceed.should.equal(true);
		protocol.Result.should.deep.equal(expectedTimeOfWork);
	});

	it("Should correct compute time of work since there is no stamps of employer for day.", function() {

		// Arrange
	
		var targetEmployerID = 1;

		// 03.04.2016
		var targetDay = new Date(2016, 3, 3);

		stamps = [];

		// Create fake source.
		var source = new FakeStampsSource.FakeStampsSource(stamps);
		var reporter = new TimeOfWorkReport(source);

		var expectedTimeOfWork = new TimeSpan.TimeSpan(0);

		// Act
		var protocol = reporter.CreateReport(targetEmployerID, targetDay); 

		// Assert
		protocol.IsSucceed.should.equal(true);
		protocol.Result.should.deep.equal(expectedTimeOfWork);
	});

	it("Should correct compute time of work since there is no stamps of employer for day.", function() {

		// Arrange
	
		var targetEmployerID = 1;

		// 03.04.2016
		var targetDay = new Date(2016, 3, 3);

		stamps = [
			// Out-stamp at 9.00 am
			new EmployerTimeStamp( targetEmployerID, StampType.Out, new Date(targetDay.setHours(9)) ),

			// Out-stamp at 10.00 am
			new EmployerTimeStamp( targetEmployerID, StampType.Out, new Date(targetDay.setHours(10)) ),

			// In-stamp at 6.00 pm
			new EmployerTimeStamp( targetEmployerID, StampType.In, new Date(targetDay.setHours(18)) ),

			// In-stamp at 6.00 pm
			new EmployerTimeStamp( targetEmployerID, StampType.In, new Date(targetDay.setHours(18)) ),
		];

		// Create fake source.
		var source = new FakeStampsSource.FakeStampsSource(stamps);
		var reporter = new TimeOfWorkReport(source);

		// Result should be like this because 
		// system makes the following changes in data:
		// 		1. Creates In-stamp at 12.00 am of target day
		// 		2. Insert In-stamp at 9.30 am between Out-stamp at 9.00 am 
		//												and Out-stamp at 10.00 am
		//		3. Delete one of In-stamp at 6.00 pm
		// 		4. Creates Out-stamps at 11.59.59:999 pm of target day
		// 	So employer come  in at 12.00 am, 
		// 	than ho out at 9.00 am (9 hours of work),
		// 	come in again at 9.30 am,
		// 	go out at 10.00 am (plus 30 minutes = 9 hours and 30 minutes of work),
		// 	come in at 6.pm and go out at 11.59.59:999 pm(plus 5.59.59:999 hours of work).
		// 	9.30 + 5.59.59:999 = 15.29.59:999 hours of work.
		var expectedTimeOfWork = new TimeSpan.TimeSpan(999, 59, 29, 15);

		// Act
		var protocol = reporter.CreateReport(targetEmployerID, targetDay);

		// Assert
		protocol.IsSucceed.should.equal(true);
		protocol.Result.should.deep.equal(expectedTimeOfWork);
	});

});