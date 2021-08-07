jQuery.browser = {};
(function () {
    jQuery.browser.msie = false;
    jQuery.browser.version = 0;
    if (navigator.userAgent.match(/MSIE ([0-9]+)\./)) {
        jQuery.browser.msie = true;
        jQuery.browser.version = RegExp.$1;
    }
})();

//jQuery.extend(jQuery.validator.methods, {
//    date: function (value, element) {
//        return this.optional(element) || /^\d\d?\/\d\d?\/\d\d\d?\d?$/.test(value);
//    },
//    number: function (value, element) {
//        return this.optional(element) || /^-?(?:\d+|\d{1,3}(?:\.\d{3})+)(?:,\d+)?$/.test(value);
//    }
//});
