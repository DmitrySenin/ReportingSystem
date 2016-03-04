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

});