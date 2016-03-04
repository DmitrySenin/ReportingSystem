var TimeSpan = require('timespan');

var messages = {
	UndefinedStartOrEndTime : "Date of start or end of respite can't be undefined.",
	ConstructorParamsNotDateInstances : "To create respite parameters should be instance of Date.",
	StartDateGreaterEndDate : "Start time is greater than end time."
};

/**
 * Create object represents Respite. This object is unchangeable.
 * @param {Date} startDate Start of respite.
 * @param {Date} endDate   End of respite.
 */
function Respite(startDate, endDate) {
	if(startDate == undefined || endDate == undefined) {
		throw new Error(messages.UndefinedStartOrEndTime);
	}

	if(!(startDate instanceof Date || endDate instanceof Date)) {
		throw new Error(messages.ConstructorParamsNotDateInstances);
	}

	if(startDate.getTime() > endDate.getTime()) {
		throw new Error(messages.StartDateGreaterEndDate);
	}

	this.StartDate = startDate;
	this.EndDate = endDate;

	this.Duration = TimeSpan.fromMilliseconds(this.EndDate - this.StartDate);

	Object.freeze(this);
}

/**
 * Compare two respites.
 * @param {Respite} x First respite to compare.
 * @param {Respite} y Second respite to compare.
 * @return {Numeric} Negative value if first less than second. 
 *                            Positive value if first greater than second. 
 *                            Zero if types are equal.
 */
Respite.Compare = function(x, y) {
	if(x.StartDate.getTime() != y.StartDate.getTime()) {
		return x.StartDate < y.StartDate ? -1 : 1;
	} else if(y.endDate.getTime() != y.endDate.getTime()) {
		return x.EndDate < y.EndDate ? -1 : 1;
	} else {
		return 0;
	}
}

module.exports = {
	Respite
};

