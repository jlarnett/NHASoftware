$(document).ready(function () {
    
    $("#HomeContentFeed").on('click', '.feed-profile-link', function (e) {
        var SendButton = $(e.target);
        var userId = SendButton.attr("userId");
        var profileUrl = "Users/GetProfiles?userId=" + userId;
        UserNavigatedToLink(profileUrl);
    });

    $("#ContentFeed").on('click', '.profile-link', function (e) {
        var SendButton = $(e.target);
        var userId = SendButton.attr("userId");
        var profileUrl = "/Users/GetProfiles?userId=" + userId;
        UserNavigatedToLink(profileUrl);
    });

});