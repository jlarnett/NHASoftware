$(document).ready(function () {
    
    $("#HomeContentFeed").on('click', '.feed-profile-link', function (e) {
        var SendButton = $(e.target);
        var userId = SendButton.attr("userId")
        window.location.replace("Users/GetProfiles?userId=" + userId);
    });

});