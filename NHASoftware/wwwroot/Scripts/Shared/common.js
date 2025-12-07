//Retrieves the currently logged in users id. Can return undefined. 
function RetrieveCurrentUserId() {
    let element = document.getElementById('MainPageContentContainer');
    return element.getAttribute('userId');
}

//Returns whether
function CheckUserSessionIsActive() {
    let element = document.getElementById('MainPageContentContainer');
    return element.getAttribute('user-session-active');
}

//Returns a bool whether the current user is an admin or not. 
function IsCurrentUserAdmin() {
    let element = document.getElementById('MainPageContentContainer');
    return element.getAttribute('admin-user');
}

//Used to navigate user to URL. Maintains forwards / Backwards history
function UserNavigatedToLink(url) {
    location.href = url;
}

//Returns a rounded shorthand string for the age of post. E.G 2H A
function GetTimeShortHandString(timeInSeconds) {

    let timeInMinutes = RoundToWholeNumber(timeInSeconds / 60);
    let timeInHours = RoundToWholeNumber(timeInMinutes / 60);
    let timeInDays = RoundToWholeNumber(timeInHours / 24);
    let timeInYears = RoundToWholeNumber(timeInDays / 365);

    var outputShorthandUnit;


    if (timeInSeconds < 60 && timeInSeconds >= 0) {
        outputShorthandUnit = (timeInSeconds > 1 ? " seconds ago" : " second ago");
        return timeInSeconds + outputShorthandUnit;
    }
    if (timeInMinutes < 60 && timeInMinutes >= 1) {

        outputShorthandUnit = (timeInMinutes > 1 ? " minutes ago" : " minute ago");
        return timeInMinutes + outputShorthandUnit;
    }
    if (timeInHours < 24 && timeInHours >= 1) {
        outputShorthandUnit = (timeInHours > 1 ? " hours ago" : " hour ago");
        return timeInHours + outputShorthandUnit;
    }

    if (timeInDays < 365 && timeInDays >= 1) {
        outputShorthandUnit = (timeInDays > 1 ? " days ago" : " day ago");
        return timeInDays + outputShorthandUnit;
    }

    if (timeInYears >= 1) {
        outputShorthandUnit = (timeInYears > 1 ? " years ago" : " year ago");
        return timeInYears + outputShorthandUnit;
    }


}

//Returns rounded whole number of parameter without excess 
function RoundToWholeNumber(number) {
    return Math.round(number);
}

function Utils() {

}

Utils.prototype = {
    constructor: Utils,
    isElementInView: function (element, fullyInView) {
        var pageTop = $(window).scrollTop();
        var pageBottom = pageTop + $(window).height();
        var elementTop = $(element).offset().top;
        var elementBottom = elementTop + $(element).height();

        if (fullyInView === true) {
            return ((pageTop < elementTop) && (pageBottom > elementBottom));
        } else {
            return ((elementTop <= pageBottom) && (elementBottom >= pageTop));
        }
    },
    containsObject: function(obj, list) {
        var i;
        for (i = 0; i < list.length; i++) {
            if (list[i] === obj) {
                return true;
            }
        }

        return false;
    }   
};

var Utils = new Utils();

//Used to make dropdowns work on hover, but keep bootstrap styling
document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll('.dropdown:not(.no-hover)').forEach(dd => {

        dd.addEventListener('mouseenter', () => {
            const toggle = dd.querySelector('[data-bs-toggle="dropdown"]');
            bootstrap.Dropdown.getOrCreateInstance(toggle).show();
        });

        dd.addEventListener('mouseleave', () => {
            const toggle = dd.querySelector('[data-bs-toggle="dropdown"]');
            bootstrap.Dropdown.getOrCreateInstance(toggle).hide();
        });

    });
});
