class ContentFeedUtility {

    static postUploadButtonText = "";

    static AddPostToContentFeedUI(postDTO) {
        ContentFeedAjaxCalls.ConvertPostDTOToPartialView(postDTO).then(function (postPartialView) {
            ContentFeedUtility.PrependPostsToContentFeed(postPartialView);
            ContentFeedUtility.RebuildFeedTextboxes();
        }).catch(function() {
            console.log("Ran into issue converting postDTO -> post partial view. AJAX Call failed")
        });
    }

    static AddCommentToContentFeedUI(uuid, commentDTO) {
        ContentFeedAjaxCalls.ConvertCommentDTOToPartialView(commentDTO).then(function (commentsPartialView) {
            ContentFeedUtility.AppendCommentsToPost(uuid, commentsPartialView);
            ContentFeedUtility.IncrementPostCommentCounter(uuid);
        }).catch(function () {
            console.log("Ran into issue converting commentDTO -> comment partial view. AJAX Call failed")
        });
    }

    static RemovePostFromContentFeedUI(postId) {
        //Removes the post from the content feed'Ss HTML. This doesn't affect the BD just HTML. 
        var postContainer = $('[post-id=' + postId + ']');
        postContainer.remove();
    }

    static RemoveCommentFromContentFeedUI(postId) {
        //Removes comment from the content feed's HTML. This doesn't affect the DB just HTML.
        var commentContainer = $('[comment-delete-id=' + postId + ']');
        commentContainer.remove();
    }

    static LoadUserProfilePosts() {
        //Tries to load content feed with user profile post only if content feed is on user profile page.
        var profileUserId = this.TryGetContentFeedUserProfileId();
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

    static LoadCommentsRedesign(postId, uuid) {
        //Loads comments for post using bootstrap redesign. Takes the post-id & uuid to determine which comment section to load
        //Called by Hide Comments js script. 

        ContentFeedAjaxCalls.RetrievePostComments(postId, uuid).then(function (comments) {
            ContentFeedUtility.AppendCommentsToPost(uuid, comments)
        });
    }

    static TryGetContentFeedUserProfileId() {
        //Returns profile-user-Id This determines whether the content feed is main feed or user profile
        return $('#ContentFeed').attr("profile-user-id");
    }

    static PrependPostsToContentFeed(posts) {
        $("#ContentFeed").prepend(posts)
    }
    static AppendPostsToContentFeed(posts) {
        $("#ContentFeed").append(posts)
    }

    static AppendCommentsToPost(uuid, comments) {
        $("ul[unique-comment-list$=" + uuid + "]").append(comments);
    }

    static HideCustomPostModal() {
        $('#AddPostPhotosModal').modal('hide');
    }

    static ShowPostImageCarousel(uuid) {
        $("#Image-Carousel-" + uuid).show();
    }

    static AddSpinnerToImageSection(uuid) {
        $("[unique-image-section=" + uuid + "]").append('<div image-loader-uuid="' + uuid + '" class="text-center mt-2"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>');
    }

    static RemoveSpinnerFromImageSection(uuid) {
        $("#ContentFeed").find('[image-loader-uuid="' + uuid + '"]').remove();
    }

    static AddSpinnerToContentFeed() {
    $("#ContentFeed").append('<div id="ContentFeedLoadingSpinner" class="text-center mt-2"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>');
}

    static RemoveSpinnerFromContentFeed() {
        $('#ContentFeedLoadingSpinner').remove();
    }

    static AddSpinnerSubmitCustomPostBtn() {
        this.postUploadButtonText = $('#SubmitCustomPostBtn').text();
        $('#SubmitCustomPostBtn').text("");
        $('#SubmitCustomPostBtn').prepend("<span id='CustomPostLoadingSpinner' class='spinner-border spinner-border' role='status' aria-hidden='true'></span>");
    }

    static AddSpinnerSubmitPostBtn() {
        this.postUploadButtonText = $('#SubmitBtn').text();
        $('#SubmitBtn').text("");
        $('#SubmitBtn').prepend("<span id='BasicPostLoadingSpinner' class='spinner-border spinner-border' role='status' aria-hidden='true'></span>");
    }

    static RemoveSpinnerSubmitCustomPostBtn() {
        $('#CustomPostLoadingSpinner').remove();
        $('#SubmitCustomPostBtn').text(this.postUploadButtonText);
    }

    static RemoveSpinnerSubmitPostBtn() {
        $('#BasicPostLoadingSpinner').remove();
        $('#SubmitBtn').text(this.postUploadButtonText);
    }

    static IncrementPostCommentCounter(uuid) {
        let CommentCounter = $("a[unique-post-id$=" + uuid + "]");
        let count = parseInt(CommentCounter.attr("comment-count"));
        CommentCounter.attr("comment-count", count + 1);
        CommentCounter.text(CommentCounter.text().replace(count, count + 1));
    }

    
    static DecrementPostCommentCounter(uuid) {
        let CommentCounter = $("a[unique-post-id$=" + uuid + "]");
        let count = parseInt(CommentCounter.attr("comment-count"));
        CommentCounter.attr("comment-count", count + 1);
        CommentCounter.text(CommentCounter.text().replace(count, count - 1));
    }

    //static AddSpinnerGeneric(id, uuid = undefined) {
    //    $("#ContentFeed").append('<div id="ContentFeedLoadingSpinner" class="text-center mt-2"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>');
    //}

    static RebuildFeedTextboxes() {
        //Used to rebuild summernote text boxes. This is needed whenever the textboxes are added dynamically to feed.

        $('.summernote-comments').summernote({
            toolbar: [
            // [groupName, [list of button]]
            ],
            disableResizeEditor: true,
            placeholder: 'Type Comment Summary Here......'
        });

        $('#MainPostTextbox').summernote({
            toolbar: [
                //['misc', ['emoji']]
            ],
            disableResizeEditor: true,
            placeholder: 'Type Post Summary Here.......'
        });

        $('#CustomPostTextbox').summernote({
            toolbar: [
            // [groupName, [list of button]]
            ],
            disableResizeEditor: true,
            placeholder: 'Type Custom Post Summary Here.....'
        });
    }


}






