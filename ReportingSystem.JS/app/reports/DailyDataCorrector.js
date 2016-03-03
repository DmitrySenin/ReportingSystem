
var StampType = require('./StampType.js');
var EmployerTimeStamp = require('./EmployerTimeStamp.js');
var NotificationType = require('./NotificationType.js');
var Notification = require('./Notification.js');
var ReportProtocol = require('./ReportProtocol.js');

var messages = {
	UndefinedSourceDataReference : "Source of data can't be null or undefined.",
	NotificationsIsNotArray : "Notifications should be an array.",
	StampsIsNotArray : "System can work only with array of time stamps.",
	SourceDataIncorrectDataFormat : "Source of stamps should return array of employer stamps.",
	UndefinedStampsCollection : "Collection of stamps can't be null or undefined.",
	FirstInStampNotFound : "First In-stamp was not found.",
	BeginDayAsInStampAdded : "Begin of target day was added as first In-stamp.",
	LastOutStampNotFound : "Last out-stamp was not found.",
	FirstOutNextDayAsLast : "First Out-stamp of next day was added as last Out-stamp.",
	EndDayAsLastOutStamp : "End of target day was added as last Out-stamp.",
	OneOfEqualStampsRemoved : "Equal stamps was found. One of them was removed.",
	NewStampInMiddleBetweenSameType : "Found two followed stamps of one type. Add new between them at the middle."
};

/**
 * Create object which collect and correct data for daily reports.
 * @param {Object} dataSource Object which contains: GetAll, GetByEmployerID, GetByEmployerIDForDay methods.
 */
function DailyDataCorrector(dataSource) {

	if(dataSource === undefined || dataSource === null) {
		throw new Error(messages.UndefinedSourceDataReference);
	}

	this._dataSource = dataSource;
}

/**
 * Gather all data for daily report, checks it and complete if necessary.
 * Add important notifications during repairing data.
 * @param {Numeric} employerID    Unique identifier of employer.
 * @param {Date} day           Reporting day.
 * @param {Array} notifications Collection of notifications to notify about important event.
 * @return {Array} Checked stamps for daily reporting.
 */
DailyDataCorrector.prototype.CollectStampsForDailyReport = function(employerID, day, notifications) {

	if(!Array.isArray(notifications)) {
		throw new Error(messages.NotificationsIsNotArray);
	}

	// Get stamps from source.
	stamps = this._dataSource.GetByEmployerIDForDay(employerID, day);

	if(!Array.isArray(stamps)) {
		throw new Error(messages.SourceDataIncorrectDataFormat);
	}

	// No stamps for day.
	if(stamps.length === 0) {
		return stamps;
	}

	this._verifyFirstStamp(stamps, employerID, day, notifications);
	this._verifyLastStamp(stamps, employerID, day, notifications);
	this._verifyStampsSequence(stamps, employerID, day, notifications);

	return stamps;
};

/**
 * Check out first element of collection of stamps 
 * to add if require non existent first In-stamp.
 * @param  {Array} stamps        Collections that should be checked.
 * @param  {Numeric} employerID    Target employer's unique identifier.
 * @param  {Date} day           Date of day of reporting.
 * @param  {Array} notifications Notifications that will be completed during processing.
 */
DailyDataCorrector.prototype._verifyFirstStamp = function(stamps, employerID, day, notifications) {

	if(!Array.isArray(notifications)) {
		throw new Error(messages.NotificationsIsNotArray);
	}

	if(stamps === undefined || stamps === null) {
		throw new Error(messages.UndefinedStampsCollection);
	}

	if(!Array.isArray(stamps)) {
		throw new Error(messages.StampsIsNotArray);
	}

	// if there is not first In-stamp
	// add it as begging of target day.
	if(stamps.length === 0 || stamps[0].Type !== StampType.In) {
		notifications.push(new Notification(messages.FirstInStampNotFound, NotificationType.Warning));
		notifications.push(new Notification(messages.BeginDayAsInStampAdded, NotificationType.Message));

		beggingOfTargetDay = new Date(day.getFullYear(), day.getMonth(), day.getDate(), 0, 0, 0, 0);

		firstInStamp = new EmployerTimeStamp(employerID, StampType.In, beggingOfTargetDay);

		stamps.unshift(firstInStamp);
	}
};

/**
 * Check out last element of collection of stamps
 * to add if require non existent last out-stamp.
 * @param  {Array} stamps        Collections that should be checked.
 * @param  {Numeric} employerID    Target employer's unique identifier.
 * @param  {Date} day           Date of day of reporting.
 * @param  {Array} notifications Notifications that will be completed during processing.
 */
DailyDataCorrector.prototype._verifyLastStamp = function(stamps, employerID, day, notifications) {
	if(!Array.isArray(notifications)) {
		throw new Error(messages.NotificationsIsNotArray);
	}

	if(stamps === undefined || stamps === null) {
		throw new Error(messages.UndefinedStampsCollection);
	}

	if(!Array.isArray(stamps)) {
		throw new Error(messages.StampsIsNotArray);
	}

	// if last stamps is not Out-stamp
	// then find stamps for next day and check first one.
	// or add end of target day as last Out-stamp.
	if(stamps.length == 0 || stamps[stamps.length - 1].Type !== StampType.Out) {
		notifications.push(new Notification(messages.LastOutStampNotFound, NotificationType.Warning));

		nextDay = new Date(day);
		nextDay.setDate(nextDay.getDate() + 1);

		stampsForNextDay = this._dataSource.GetByEmployerIDForDay(employerID, nextDay);
		nextDayFindingDate = this._nextDayEarliestFindingTime(day);

		// there is stamps for next day
		// and first one is Out-stamp with time less or equal restricting time
		// then add this one as last out stamp
		// else add end of target day as last out stamp
		if(stampsForNextDay != undefined 
				&& stampsForNextDay.length > 0
				&& stampsForNextDay[0].Type == StampType.Out 
				&& stampsForNextDay[0].Time <= nextDayFindingDate) {

			notifications.push(new Notification(messages.FirstOutNextDayAsLast, NotificationType.Message));

			stamps.push(stampsForNextDay[0]);

		} else {

			notifications.push(new Notification(messages.EndDayAsLastOutStamp, NotificationType.Message));

			nextDay.setHours(23, 59, 59, 999);

			lastOutStamp = new EmployerTimeStamp(employerID, StampType.Out, nextDay);

			stamps.push(lastOutStamp);
		}
	}
};

/**
 * Check out sequence of stamps in collection
 * that each In-stamp is followed by Out-stamp.
 * @param  {Array} stamps        Collections that should be checked.
 * @param  {Numeric} employerID    Target employer's unique identifier.
 * @param  {Date} day           Date of day of reporting.
 * @param  {Array} notifications Notifications that will be completed during processing.
 */
DailyDataCorrector.prototype._verifyStampsSequence = function(stamps, employerID, day, notifications) {

	if(!Array.isArray(notifications)) {
		throw new Error(messages.NotificationsIsNotArray);
	}

	if(stamps === undefined || stamps === null) {
		throw new Error(messages.UndefinedStampsCollection);
	}

	if(!Array.isArray(stamps)) {
		throw new Error(messages.StampsIsNotArray);
	}

	i = 0;
	while(i < stamps.length - 1) {

		// If two consecutive stamps have equal type.
		// Then compare time and delete one of the, if times are equal
		// else insert stamp between them in the middle. 
		if(!StampType.Compare(stamps[i].Type, stamps[i + 1].Type)) {

			// Stamps' times are equal.
			if(stamps[i].Time.getTime() === stamps[i + 1].Time.getTime()) {
				notifications.push(new Notification(messages.OneOfEqualStampsRemoved, NotificationType.Warning));

				stamps.splice(i + 1, 1);

				// Indexer should not be changed because 
                // new (i + 1) element can be same type and time again.
                // So we need to compare i-th and (i + 1)-th again.
			} else {
				notifications.push(new Notification(messages.NewStampInMiddleBetweenSameType, NotificationType.Warning));

				diffInSeconds = stamps[i + 1].Time - stamps[i].Time;

				// Compute time in the middle of stamps.
				// Expression inside parentheses computes seconds to create middle date. 
				middleDate = new Date(Number(stamps[i].Time) + diffInSeconds / 2);

				middleStampType = StampType.Compare(stamps[i].Type, StampType.In) ? StampType.Out : StampType.In;
				middleStamp = new EmployerTimeStamp(employerID, middleStampType, middleDate);

				// Insert middle stamp stamp to (i + 1) place.
				stamps.splice(i + 1, 0, middleStamp);

				// Go to element that was (i + 1) before insert.
				i += 2;
			}
		} else {
			// Go to next element to check.
			i++;
		}
	}
};

/**
 * Build time of employer stamps to find records in next day.
 * @param  {Date} day Date of day for which finding date will be built.
 * @return {Date}     Time to restrict finding of data.
 */
DailyDataCorrector.prototype._nextDayEarliestFindingTime = function (day) {
	// 4 am of next day.
	nextDay = new Date(day);
	nextDay.setDate(nextDay.getDate() + 1);
	nextDay.setHours(4, 0, 0, 0);
};

module.exports = DailyDataCorrector;