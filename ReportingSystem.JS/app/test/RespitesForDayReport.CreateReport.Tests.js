var should = require('chai').should();
var TimeSpan = require('timespan');

var StampType = require('../reports/StampType.js');
var EmployerTimeStamp = require('../reports/EmployerTimeStamp.js');
var Respite = require('../reports/Respite.js');
var RespitesForDayReport = require('../reports/RespitesForDayReport.js');

var FakeStampsSource = require('./FakeStampsSource.js');

describe("RespitesForDayReport.CreateReport", function() {

	it("Should find all respites since data is correct.", function() {

		var targetEmployerID = 1;

		// 03.04.2016
		var targetDay = new Date(2016, 3, 3);

		var maxRespiteDuration = TimeSpan.fromMinutes(15); 

		stamps = [
			// In-stamp at 9.00 am
			new EmployerTimeStamp( targetEmployerID, StampType.In, new Date(targetDay.setHours(9)) ),

			// Out-stamp at 10.00 am
			new EmployerTimeStamp( targetEmployerID, StampType.Out, new Date(targetDay.setHours(10)) ),

			// In-stamp at 10.14 pm
			new EmployerTimeStamp( targetEmployerID, StampType.In, new Date(targetDay.setHours(10, 14)) ),

			// Out-stamp at 6.00 pm
			new EmployerTimeStamp( targetEmployerID, StampType.Out, new Date(targetDay.setHours(18)) ),
		];

		// Create fake source.
		var source = new FakeStampsSource.FakeStampsSource(stamps);
		var reporter = new RespitesForDayReport.RespitesForDay(source);

		// Expected respite between
		// Out-stamp at 10.00 am and In-stamp at 10.14 am.
		var expectedRespites = [
			new Respite.Respite(stamps[1].Time, stamps[2].Time)
		];

		var protocol = reporter.CreateReport(targetEmployerID, targetDay, maxRespiteDuration); 

		protocol.IsSucceed.should.equal(true);
		protocol.Result.should.deep.equal(expectedRespites);

	});

	it("Should not find any respites since stamps collection is empty.", function() {

		var targetEmployerID = 1;

		// 03.04.2016
		var targetDay = new Date(2016, 3, 3);

		var maxRespiteDuration = TimeSpan.fromMinutes(15); 

		stamps = [];

		// Create fake source.
		var source = new FakeStampsSource.FakeStampsSource(stamps);
		var reporter = new RespitesForDayReport.RespitesForDay(source);

		// No expected respites 
		var expectedRespites = [];

		var protocol = reporter.CreateReport(targetEmployerID, targetDay, maxRespiteDuration); 

		protocol.IsSucceed.should.equal(true);
		protocol.Result.should.deep.equal(expectedRespites);

	});

	it("Should not find any respites since maximum respite duration equals 0.", function() {

		var targetEmployerID = 1;

		// 03.04.2016
		var targetDay = new Date(2016, 3, 3);

		var maxRespiteDuration = new TimeSpan.TimeSpan(0); 

		stamps = [
			// In-stamp at 9.00 am
			new EmployerTimeStamp( targetEmployerID, StampType.In, new Date(targetDay.setHours(9)) ),

			// Out-stamp at 10.00 am
			new EmployerTimeStamp( targetEmployerID, StampType.Out, new Date(targetDay.setHours(10)) ),

			// In-stamp at 10.01 am
			new EmployerTimeStamp( targetEmployerID, StampType.In, new Date(targetDay.setHours(10)) ),

			// Out-stamp at 6.00 pm
			new EmployerTimeStamp( targetEmployerID, StampType.Out, new Date(targetDay.setHours(18)) ),
		];

		// Create fake source.
		var source = new FakeStampsSource.FakeStampsSource(stamps);
		var reporter = new RespitesForDayReport.RespitesForDay(source);

		// No expected respites 
		// Source of data should sort stamp by its type too
		// so In-stamp at 10.00 am will at the 1st position(start with 0)
		// and Out-stamp at 10.00 am will at the 2nd position
		// so system will not compute respite using them 
		// because respite is computed between Out and In stamps in the such order.
		var expectedRespites = [];

		var protocol = reporter.CreateReport(targetEmployerID, targetDay, maxRespiteDuration);

		console.log(protocol.Notifications);

		protocol.IsSucceed.should.equal(true);
		protocol.Result.should.deep.equal(expectedRespites);

	});

});