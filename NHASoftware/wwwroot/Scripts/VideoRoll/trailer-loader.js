$(window).on("load", function () {

    $(window).on("scroll", function () {
        //Called whenever the user scrolls the document. Handles loading more post for infinite feed loop
        //Handles loading images as the user scrolls the feed. This keeps the base load time for post faster
        if (ContentFeedUtility.TryGetContentFeedUserProfileId() === undefined) {
            ContentFeedLoader.ShouldContentFeedShouldLoadMorePosts();
        }

        ImageLoader.ShouldPostLoadImagesFromDB();
    });
});

class VideoFeedLoader {
    static canLoad = true;

    static ShouldContentFeedShouldLoadMorePosts() {
        //Checks the home page scroll bar. When the scrollbar is lower than the specified percentage it fires home feed content loading.
        var scrollbarValue = $(window).scrollTop() + $(window).height();
        var windowHeightPercentToLoadFeed = 55;
        var windowHeight = Math.trunc($(document).height());
        var percentScrolled = Math.trunc((scrollbarValue / windowHeight) * 100);

        const delay = 5000; // 5 seconds

        //Debugging Log
        //console.log("Percent Scrolled - " + percentScrolled);

        if((percentScrolled >= windowHeightPercentToLoadFeed || percentScrolled == 100)) {

            if (this.canLoad) {
                this.canLoad = false;
                this.OptimizedMainContentFeedLoad();
                setTimeout(function () {ContentFeedLoader.canLoad = true;}, delay)
            }
        }
    }

}
