var EmployerTimeStamp = require('../reports/EmployerTimeStamp.js');

/**
 * Create object which can be used as source of stamps.
 * @param {Array} stamps Array of stamps which will be used by instance.
 */
function FakeStampsSource(stamps) {
	this._stamps = stamps;
}

/**
 * Collect data of employer for a day.
 * @param {Numeric} employerID Unique identifier of employer.
 * @param {Date} day        Date of target day.
 */
FakeStampsSource.prototype.GetByEmployerIDForDay = function(employerID, day) {
	return this._stamps
					.filter(function(item) {
						return item.EmployerID === employerID
								&& item.Time.getFullYear() === day.getFullYear()
								&& item.Time.getMonth() === day.getMonth()
								&& item.Time.getDate() === day.getDate();
					})
					.sort(EmployerTimeStamp.Compare);
}

module.exports = {
	FakeStampsSource
};

