/**
 * Create notification classifier.
 * @param {Numeric} Id Unique identifier of type.
 * @param {String} Name Description of type.
 */
function NotificationType(Id, Name) {
	this.Id = ++Id;
	this.Name = Name + '';
	Object.freeze(this);
}

/**
 * Convert notification type to number.
 * @return {Numeric} Id of type;
 */
NotificationType.prototype.valueOf = function() {
	return this.Id;
};

/**
 * Convert notification type to string.
 * @return {String} Name of type.
 */
NotificationType.prototype.toString = function() {
	return this.Name;
};

/**
 * Compare two notification type.
 * @param {NotificationType} x First notification type to compare.
 * @param {NotificationType} y Second notification type to compare.
 * @return {Numeric} Negative value if first less than second. 
 *                            Positive value if first greater than second. 
 *                            Zero if types are equal.
 */
NotificationType.Compare = function(x, y) {
	return x.Id - y.Id;
};

// Notification is error.
NotificationType.Error = new NotificationType(0, 'Error');

// Notification is warning
NotificationType.Warning = new NotificationType(1, 'Warning');

// Notification is message.
NotificationType.Message = new NotificationType(2, 'Message');

module.exports = {
	Error: NotificationType.Error,
	Warning: NotificationType.Warning,
	Message: NotificationType.Message,
	Compare: NotificationType.Compare
};