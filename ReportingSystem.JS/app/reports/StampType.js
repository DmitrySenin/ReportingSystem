/**
 * Create time stamp classifier.
 * @param {Numeric} Id Unique identifier of type.
 * @param {String} Name Description of type.
 */
function StampType(Id, Name) {
	this.Id = ++Id;
	this.Name = Name + '';
	Object.freeze(this);
}

/**
 * Convert stamp type to number.
 * @return {Numeric} Id of type;
 */
StampType.prototype.valueOf = function() {
	return this.Id;
};

/**
 * Convert stamp type to string.
 * @return {String} Name of type.
 */
StampType.prototype.toString = function() {
	return this.Name;
};

/**
 * Compare two stamp type.
 * @param {StampType} x First stamp type to compare.
 * @param {StampType} y Second stamp type to compare.
 * @return {Numeric} Negative value if first less than second. 
 *                            Positive value if first greater than second. 
 *                            Zero if types are equal.
 */
StampType.Compare = function(x, y) {
	return x.Id - y.Id;
};

// Create In-type of stamp.
StampType.In = new StampType(0, 'In');

// Create Out-type of stamp.
StampType.Out = new StampType(1, 'Out');

module.exports = {
	In: StampType.In,
	Out: StampType.Out,
	Compare: StampType.Compare
};