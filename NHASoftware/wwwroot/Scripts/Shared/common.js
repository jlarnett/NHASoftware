function RetrieveCurrentUserId() {
    var element = document.getElementById('MainPageContentContainer');
    return element.getAttribute('userId');
}

function UserNavigatedToLink(url) {
    location.href = url;
}