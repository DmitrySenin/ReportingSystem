var should = require('chai').should();

var DailyDataCorrector = require('../reports/DailyDataCorrector.js');

describe("DailyDataCorrector", function() {

	// Tests of constructor.
	describe("DailyDataCorrector", function() {
		
		it("Should throw error if source of data is undefined.", function() {
			var dataSource = undefined;
			(() => new DailyDataCorrector(dataSource)).should.throw(Error);
		});

		it("Should throw error if source of data is null.", function() {
			var dataSource = null;
			(() => new DailyDataCorrector(dataSource)).should.throw(Error);
		});
	});
});