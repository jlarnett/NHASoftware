//Retrieves the currently logged in users id. Can return undefined. 
function RetrieveCurrentUserId() {
    var element = document.getElementById('MainPageContentContainer');
    return element.getAttribute('userId');
}

//Returns whether
function CheckUserSessionIsActive() {
    var element = document.getElementById('MainPageContentContainer');
    return element.getAttribute('user-session-active');
}

//Returns a bool whether the current user is an admin or not. 
function IsCurrentUserAdmin() {
    element = document.getElementById('MainPageContentContainer');
    return element.getAttribute('admin-user');
}

//Used to navigate user to URL. Maintains forwards / Backwards history
function UserNavigatedToLink(url) {
    location.href = url;
}

//Returns a rounded shorthand string for the age of post. E.G 2H A
function GetTimeShortHandString(timeInSeconds) {

    var timeInMinutes = RoundToWholeNumber(timeInSeconds / 60);
    var timeInHours = RoundToWholeNumber(timeInMinutes / 60);
    var timeInDays = RoundToWholeNumber(timeInHours / 24);
    var timeInYears = RoundToWholeNumber(timeInDays / 365);

    var outputShorthandUnit;


    if (timeInSeconds < 60 && timeInSeconds >= 1) {
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
