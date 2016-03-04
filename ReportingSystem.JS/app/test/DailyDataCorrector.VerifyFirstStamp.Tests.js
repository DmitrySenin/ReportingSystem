var should = require('chai').should();

var StampType = require('../reports/StampType.js');
var EmployerTimeStamp = require('../reports/EmployerTimeStamp.js');
var DailyDataCorrector = require('../reports/DailyDataCorrector.js');

var FakeStampsSource = require('./FakeStampsSource.js');

describe("DailyDataCorrector.VerifyFirstStamp", function() {

	it("Should add begging of target day as first In-stamp if there is no one.", function() {

		var targetEmployerID = 1;

		// 03.04.2016
		var targetDay = new Date(2016, 3, 3);

		stamps = [
			// Out-stamp at 9.00 am
			new EmployerTimeStamp( targetEmployerID, StampType.Out, new Date(targetDay.setHours(9)) )
		];

		// Create fake source.
		var source = { 
			GetByEmployerIDForDay() {
				return stamps;
			}
		};

		var dailyDataCorrector = new DailyDataCorrector(source);
		var notifications = [];

		var expectedSize = stamps.length + 1;
		var expectedStamp = new EmployerTimeStamp(targetEmployerID, StampType.In, new Date(targetDay.setHours(0)));

		dailyDataCorrector._verifyFirstStamp(stamps, targetEmployerID, targetDay, notifications);

		stamps.length.should.equal(expectedSize);
		stamps[0].should.deep.equal(expectedStamp);
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

		(() => dailyDataCorrector._verifyFirstStamp(stamps, targetEmployerID, targetDay, notifications)).should.not.throw(Error);
	});

	it("Should not make any change since data is correct.", function() {

		var targetEmployerID = 1;

		// 03.04.2016
		var targetDay = new Date(2016, 3, 3);

		stamps = [
			// In-stamp at 9.00 am
			new EmployerTimeStamp( targetEmployerID, StampType.In, new Date(targetDay.setHours(9)) ),

			// Out-stamp at 12.00 am
			new EmployerTimeStamp( targetEmployerID, StampType.Out, new Date(targetDay.setHours(12)) ),

			// In-stamp at 15.00 am
			new EmployerTimeStamp( targetEmployerID, StampType.In, new Date(targetDay.setHours(15)) ),

			// Out-stamp at 6.00 pm
			new EmployerTimeStamp( targetEmployerID, StampType.Out, new Date(targetDay.setHours(18)) )
		];

		// Create fake source.
		var source = new FakeStampsSource.FakeStampsSource(stamps);

		var dailyDataCorrector = new DailyDataCorrector(source);
		var notifications = [];

		// Copy
		var expectedStamps = stamps.slice();

		dailyDataCorrector._verifyFirstStamp(stamps, targetEmployerID, targetDay, notifications);

		stamps.should.deep.equal(expectedStamps);
	});
	
});