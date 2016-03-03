
var ReportProtocol = require('ReportProtocol.js');

var SourceDataReferenceIsUndefined = "Source of data can't be null or undefined.";
var NotificationsIsNotArray = "Notifications should be array.";
var SourceDataIncorrectDataFormat = "Source of stamps should return array of employer stamps.";

/**
 * Create object which collect and correct data for daily reports.
 * @param {Object} dataSource Object which contains: GetAll, GetByEmployerID, GetByEmployerIDForDay methods.
 */
function DailyDataCorrector(dataSource) {
	
	if(dataSource === undefined || dataSource === null) {
		throw new Error(SourceDataReferenceIsUndefined);
	}

	/**
	 * Gather all data for daily report, checks it and complete if necessary.
	 * Add important notifications during repairing data.
	 * @param {Numeric} employerID    Unique identifier of employer.
	 * @param {Date} day           Reporting day.
	 * @param {Array} notifications Collection of notifications to notify about important event.
	 * @return {Array} Checked stamps for daily reporting.
	 */
	this.CollectStampsForDailyReport = function(employerID, day, notifications) {

		if(!Array.isArray(notifications)) {
			throw new Error(NotificationsIsNotArray);
		}

		// Get stamps from source.
		stamps = dataSource.GetByEmployerIDForDay(employerID, day);

		if(Array.isArray(stamps)) {
			throw new Error(SourceDataIncorrectDataFormat);
		}

		// No stamps for day.
		if(stamps.length === 0) {
			return stamps;
		}

		this._verifyFirstStamp(stamps, employerID, day, notifications);
		this._verifyLastStamp(stamps, employerID, day, notifications);
		this._verifyStampsSequence(stamps, employerID, day, notifications);

		return stamps;
	}

	/**
	 * Check out first element of collection of stamps 
     * to add if require non existent first In-stamp.
	 * @param  {Array} stamps        Collections that should be checked.
	 * @param  {Numeric} employerID    Target employer's unique identifier.
	 * @param  {Date} day           Date of day of reporting.
	 * @param  {Array} notifications Notifications that will be completed during processing.
	 */
	this._verifyFirstStamp = function(stamps, employerID, day, notifications) {
		if(!Array.isArray(notifications)) {
			throw new Error(NotificationsIsNotArray);
		}
	}

	/**
	 * Check out last element of collection of stamps
	 * to add if require non existent last out-stamp.
	 * @param  {Array} stamps        Collections that should be checked.
	 * @param  {Numeric} employerID    Target employer's unique identifier.
	 * @param  {Date} day           Date of day of reporting.
	 * @param  {Array} notifications Notifications that will be completed during processing.
	 */
	this._verifyLastStamp = function(stamps, employerID, day, notifications) {
		if(!Array.isArray(notifications)) {
			throw new Error(NotificationsIsNotArray);
		}
	}

	/**
	 * Check out sequence of stamps in collection
	 * that each In-stamp is followed by Out-stamp.
	 * @param  {Array} stamps        Collections that should be checked.
	 * @param  {Numeric} employerID    Target employer's unique identifier.
	 * @param  {Date} day           Date of day of reporting.
	 * @param  {Array} notifications Notifications that will be completed during processing.
	 */
	this._verifyStampsSequence = function(stamps, employerID, day, notifications) {
		if(!Array.isArray(notifications)) {
			throw new Error(NotificationsIsNotArray);
		}
	}
}

module.exports = DailyDataCorrector;