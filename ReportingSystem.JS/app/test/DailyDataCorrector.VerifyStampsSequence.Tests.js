var should = require('chai').should();

var StampType = require('../reports/StampType.js');
var EmployerTimeStamp = require('../reports/EmployerTimeStamp.js');
var DailyDataCorrector = require('../reports/DailyDataCorrector.js');

var FakeStampsSource = require('./FakeStampsSource.js');

describe("DailyDataCorrector.VerifyStampsSequence", function() {

	it("Should add stamps at the middle of same typed ones.", function() {

		var targetEmployerID = 1;

		// 03.04.2016
		var targetDay = new Date(2016, 3, 3);

		stamps = [
			// Out-stamp at 9.00 am
			new EmployerTimeStamp( targetEmployerID, StampType.Out, new Date(targetDay.setHours(9)) ),

			// Out-stamp at 10.00 am
			new EmployerTimeStamp( targetEmployerID, StampType.Out, new Date(targetDay.setHours(10)) ),
		];

		// Create fake source.
		var source = new FakeStampsSource.FakeStampsSource(stamps);

		var dailyDataCorrector = new DailyDataCorrector(source);
		var notifications = [];

		var expectedSize = stamps.length + 1;
		var expectedStamps = [
			stamps[0],

			// In-stamp at 9.30 am
			new EmployerTimeStamp( targetEmployerID, StampType.Out, new Date(targetDay.setHours(9, 30)) ),

			stamps[1]
		];

		dailyDataCorrector._verifyStampsSequence(stamps, targetEmployerID, targetDay, notifications);

		stamps.length.should.equal(expectedSize);
		stamps.should.deep.equal(expectedStamps);
	});

	it("Should delete one of two equal stamps.", function() {

		var targetEmployerID = 1;

		// 03.04.2016
		var targetDay = new Date(2016, 3, 3);

		stamps = [
			// Out-stamp at 9.30 am
			new EmployerTimeStamp( targetEmployerID, StampType.Out, new Date(targetDay.setHours(9, 30)) ),

			// Out-stamp at 9.30 am
			new EmployerTimeStamp( targetEmployerID, StampType.Out, new Date(targetDay.setHours(9, 30)) ),
		];

		// Create fake source.
		var source = new FakeStampsSource.FakeStampsSource(stamps);

		var dailyDataCorrector = new DailyDataCorrector(source);
		var notifications = [];

		var expectedSize = stamps.length - 1;
		var expectedStamps = [
			stamps[0]
		];

		dailyDataCorrector._verifyStampsSequence(stamps, targetEmployerID, targetDay, notifications);

		stamps.length.should.equal(expectedSize);
		stamps.should.deep.equal(expectedStamps);
	});
	
	it("Should not throw any error if there is no stamps for day.", function() {

		var targetEmployerID = 1;

		// 03.04.2016
		var targetDay = new Date(2016, 3, 3);

		stamps = [];

		// Create fake source.
		var source = new FakeStampsSource.FakeStampsSource(stamps);

		var dailyDataCorrector = new DailyDataCorrector(source);
		var notifications = [];

		(() => dailyDataCorrector._verifyStampsSequence(stamps, targetEmployerID, targetDay, notifications)).should.not.throw(Error);
	});
	
});