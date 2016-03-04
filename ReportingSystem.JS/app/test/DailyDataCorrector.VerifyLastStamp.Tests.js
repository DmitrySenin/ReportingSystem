var should = require('chai').should();

var StampType = require('../reports/StampType.js');
var EmployerTimeStamp = require('../reports/EmployerTimeStamp.js');
var DailyDataCorrector = require('../reports/DailyDataCorrector.js');

var FakeStampsSource = require('./FakeStampsSource.js');

describe("DailyDataCorrector.VerifyLastStamp", function() {

	it("Should add end of target day as last Out-stamp if there is no one.", function() {

		var targetEmployerID = 1;

		// 03.04.2016
		var targetDay = new Date(2016, 3, 3);

		stamps = [
			// In-stamp at 9.00 am
			new EmployerTimeStamp( targetEmployerID, StampType.In, new Date(targetDay.setHours(9)) )
		];

		// Create fake source.
		var source = new FakeStampsSource.FakeStampsSource(stamps);

		var dailyDataCorrector = new DailyDataCorrector(source);
		var notifications = [];

		var expectedSize = stamps.length + 1;
		var expectedStamp = new EmployerTimeStamp(targetEmployerID, StampType.Out, new Date(targetDay.setHours(23, 59, 59, 999)));

		dailyDataCorrector._verifyLastStamp(stamps, targetEmployerID, targetDay, notifications);

		stamps.length.should.equal(expectedSize);
		stamps[stamps.length - 1].should.deep.equal(expectedStamp);
	});

	it("Should add first Out-stamp of next day since its satisfies criteria.", function() {
		var targetEmployerID = 1;

		// 03.04.2016
		var targetDay = new Date(2016, 3, 3);

		var nextDay = new Date(targetDay);
		nextDay.setDate(nextDay.getDate() + 1);

		// All stamps in source.
		allStamps = [
			// In-stamp at 9.00 am.
			new EmployerTimeStamp( targetEmployerID, StampType.In, new Date(targetDay.setHours(9)) ),

			// Out-stamp at 3.59 am of next day.
			new EmployerTimeStamp( targetEmployerID, StampType.Out, new Date(nextDay.setHours(3, 59)) ),
		];

		// Stamps of target employer for target day.
		targetDayStamps = [
			allStamps[0]
		];

		// Create fake source.
		var source = new FakeStampsSource.FakeStampsSource(allStamps);

		var dailyDataCorrector = new DailyDataCorrector(source);
		var notifications = [];

		var expectedSize = targetDayStamps.length + 1;
		var expectedStamp = allStamps[1];

		dailyDataCorrector._verifyLastStamp(targetDayStamps, targetEmployerID, targetDay, notifications);

		targetDayStamps.length.should.equal(expectedSize);
		targetDayStamps[targetDayStamps.length - 1].should.deep.equal(expectedStamp);
	});

	it("Should add end of target day since first Out-stamp of next day does not satisfy restriction.", function() {
		var targetEmployerID = 1;

		// 03.04.2016
		var targetDay = new Date(2016, 3, 3);

		var nextDay = new Date(targetDay);
		nextDay.setDate(nextDay.getDate() + 1);

		// All stamps in source.
		allStamps = [
			// In-stamp at 9.00 am.
			new EmployerTimeStamp( targetEmployerID, StampType.In, new Date(targetDay.setHours(9)) ),

			// Out-stamp at 5.00 am of next day.
			new EmployerTimeStamp( targetEmployerID, StampType.Out, new Date(nextDay.setHours(5, 00)) ),
		];

		// Stamps of target employer for target day.
		targetDayStamps = [
			allStamps[0]
		];

		// Create fake source.
		var source = new FakeStampsSource.FakeStampsSource(allStamps);

		var dailyDataCorrector = new DailyDataCorrector(source);
		var notifications = [];

		var expectedSize = targetDayStamps.length + 1;
		var expectedStamp = new EmployerTimeStamp(targetEmployerID, StampType.Out, new Date(targetDay.setHours(23, 59, 59, 999)));;

		dailyDataCorrector._verifyLastStamp(targetDayStamps, targetEmployerID, targetDay, notifications);

		targetDayStamps.length.should.equal(expectedSize);
		targetDayStamps[targetDayStamps.length - 1].should.deep.equal(expectedStamp);
	});
});