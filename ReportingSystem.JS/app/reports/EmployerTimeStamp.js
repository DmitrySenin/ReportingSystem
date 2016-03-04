var StampType = require('./StampType.js');

/**
 * Create instance of object that represents stamp of employer.
 * @param {Numeric} employerID Unique identifier of employer.
 * @param {StampType} type       Type of stamp.
 * @param {Date} time       Time of stamp
 */
function EmployerTimeStamp(employerID, type, time) {
	this.EmployerID = employerID;
	this.Type = type;
	this.Time = time;
}

/**
 * Compare two stamps of employer.
 * @param {EmployerTimeStamp} x First stamp to compare.
 * @param {EmployerTimeStamp} y Second stamp to compare.
 * @return {Numeric} Negative value if first less than second. 
 *                            Positive value if first greater than second. 
 *                            Zero if types are equal.
 */
EmployerTimeStamp.Compare = function (x, y) {
	if(x.EmployerID !== y.EmployerID) {
		return x.EmployerID - y.EmployerID;
	} else {
		if(x.Time.getTime() !== y.Time.getTime()) {
			return x.Time - y.Time;
		} else {
			return StampType.Compare(x.Type, y.Type);
		}
	}
};

module.exports = EmployerTimeStamp;