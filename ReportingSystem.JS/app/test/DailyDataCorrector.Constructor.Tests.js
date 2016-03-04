var should = require('chai').should();

var StampType = require('../reports/StampType.js');
var EmployerTimeStamp = require('../reports/EmployerTimeStamp.js');
var DailyDataCorrector = require('../reports/DailyDataCorrector.js');

var FakeStampsSource = require('./FakeStampsSource.js');

describe("DailyDataCorrector.Constructor", function() {

	it("Should throw error if source of data is undefined.", function() {
		var dataSource = undefined;
		(() => new DailyDataCorrector(dataSource)).should.throw(Error);
	});

	it("Should throw error if source of data is null.", function() {
		var dataSource = null;
		(() => new DailyDataCorrector(dataSource)).should.throw(Error);
	});
	
});