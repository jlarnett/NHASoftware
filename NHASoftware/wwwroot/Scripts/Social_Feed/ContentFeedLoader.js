$(window).on("load", function () {

    $(window).on("scroll", function() {
        //Called whenever the user scrolls the document. Handles loading more post for infinite feed loop
        //Handles loading images as the user scrolls the feed. This keeps the base load time for post faster
        if (ContentFeedUtility.TryGetContentFeedUserProfileId() === undefined) {
            ContentFeedLoader.ShouldContentFeedShouldLoadMorePosts();
        }

        ImageLoader.ShouldPostLoadImagesFromDB();
    });

    ContentFeedLoader.LoadUserProfilePosts();
    ContentFeedUtility.RebuildFeedTextboxes();
});

class ContentFeedLoader {
    static canLoad = true;

    static ShouldContentFeedShouldLoadMorePosts() {
        const contentFeed = $("#ContentFeed");
        if (contentFeed.attr("data-has-more") === "false") {
            return;
        }

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

    static OptimizedMainContentFeedLoad() {
        //Loads the id #ContentFeed with all posts created by user. Calls Home Base Controller (Simplifies Partial View Return)
        const contentFeed = $("#ContentFeed");
        const currentPage = parseInt(contentFeed.attr("data-current-page") ?? "1", 10);
        const pageSize = parseInt(contentFeed.attr("data-page-size") ?? "10", 10);
        const nextPage = currentPage + 1;

        ContentFeedUtility.AddSpinnerToContentFeed();
        ContentFeedAjaxCalls.RetrieveMorePosts(nextPage, pageSize)
            .then(function (posts) {
                const trimmedPosts = posts?.trim() ?? "";

                if (trimmedPosts.length === 0) {
                    contentFeed.attr("data-has-more", "false");
                    return;
                }

                contentFeed.attr("data-current-page", nextPage);
                ContentFeedUtility.AppendPostsToContentFeed(posts);
                ContentFeedUtility.RebuildFeedTextboxes();
            })
            .always(function () {
                ContentFeedUtility.RemoveSpinnerFromContentFeed();
            });
    }

    static LoadUserProfilePosts() {
        //Tries to load content feed with user profile post only if content feed is on user profile page.
        var profileUserId = ContentFeedUtility.TryGetContentFeedUserProfileId();
        if (profileUserId !== undefined) {
            this.LoadFeedWithProfilePost(profileUserId);
        }
    }

    static LoadFeedWithProfilePost(userId) {
        //Loads the id #ContentFeed with all posts created by user. Calls Post WebAPI
        ContentFeedUtility.AddSpinnerToContentFeed();
        ContentFeedAjaxCalls.RetrieveAllPostForUser(userId).then(function (posts) {
            ContentFeedUtility.AppendPostsToContentFeed(posts);
            ContentFeedUtility.RebuildFeedTextboxes();
            ContentFeedUtility.RemoveSpinnerFromContentFeed();
        });
    }
}


