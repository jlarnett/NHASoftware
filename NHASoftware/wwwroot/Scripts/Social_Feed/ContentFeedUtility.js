class ContentFeedUtility {

    static postUploadButtonText = "";

    static AddPostToContentFeedUI(postDTO) {
        //Takes input of postDTO. Calls the Controller to convert to partial view since its dynamic. 
        ContentFeedAjaxCalls.ConvertPostDTOToPartialView(postDTO).then(function (postPartialView) {
            ContentFeedUtility.PrependPostsToContentFeed(postPartialView);
            ContentFeedUtility.RebuildFeedTextboxes();
        }).catch(function() {
            console.log("Ran into issue converting postDTO -> post partial view. AJAX Call failed")
        });
    }

    static AddCommentToContentFeedUI(uuid, commentDTO) {
        //Takes input of commentDTO. Calls the Controller to convert to partial view since its dynamic. 
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
        //Called by Hide Comments js script

        ContentFeedAjaxCalls.RetrievePostComments(postId, uuid).then(function (comments) {
            ContentFeedUtility.AppendCommentsToPost(uuid, comments)
        });
    }

    static TryGetContentFeedUserProfileId() {
        //Returns profile-user-Id This determines whether the content feed is main feed or user profile
        return $('#ContentFeed').attr("profile-user-id");
    }

    static PrependPostsToContentFeed(posts) {
        //Prepends post to content feed. E.G. post partial view
        $("#ContentFeed").prepend(posts)
    }
    static AppendPostsToContentFeed(posts) {
        //Appends post to content feed. E.G. post partial view
        $("#ContentFeed").append(posts)
    }

    static AppendCommentsToPost(uuid, comments) {
        //Appends commentPartialView "comments" to supplied uuid post
        $("ul[unique-comment-list$=" + uuid + "]").append(comments);
    }

    static HideCustomPostModal() {
        //Hides the Custom post modal
        $('#AddPostPhotosModal').modal('hide');
    }

    static ShowPostImageCarousel(uuid) {
        //Makes the image carousel on posts visible for specified uuid.
        $("#Image-Carousel-" + uuid).show();
    }

    static AddSpinnerToImageSection(uuid) {
        //Adds loading spinner to the image section of specified post uuid
        $("[unique-image-section=" + uuid + "]").append('<div image-loader-uuid="' + uuid + '" class="text-center mt-2"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>');
    }

    static RemoveSpinnerFromImageSection(uuid) {
        //Removes loading spinner to the image section of specified post uuid
        $("#ContentFeed").find('[image-loader-uuid="' + uuid + '"]').remove();
    }

    static AddSpinnerToContentFeed() {
    //Adds loading spinner to the Content Feed
        $("#ContentFeed").append('<div id="ContentFeedLoadingSpinner" class="text-center mt-2"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>');
    }

    static RemoveSpinnerFromContentFeed() {
        //Removes loading spinner from Content Feed
        $('#ContentFeedLoadingSpinner').remove();
    }

    static AddSpinnerSubmitCustomPostBtn() {
        //Adds loading spinner to submit custom post btn. 
        this.postUploadButtonText = $('#SubmitCustomPostBtn').text();
        $('#SubmitCustomPostBtn').text("");
        $('#SubmitCustomPostBtn').prepend("<span id='CustomPostLoadingSpinner' class='spinner-border spinner-border' role='status' aria-hidden='true'></span>");
    }

    static AddSpinnerSubmitPostBtn() {
        //Add loading spinner to basic post submit button. 
        this.postUploadButtonText = $('#SubmitBtn').text();
        $('#SubmitBtn').text("");
        $('#SubmitBtn').prepend("<span id='BasicPostLoadingSpinner' class='spinner-border spinner-border' role='status' aria-hidden='true'></span>");
    }

    static RemoveSpinnerSubmitCustomPostBtn() {
        //Remove loading spinner from custom post submit button. 
        $('#CustomPostLoadingSpinner').remove();
        $('#SubmitCustomPostBtn').text(this.postUploadButtonText);
    }

    static RemoveSpinnerSubmitPostBtn() {
        //Remove loading spinner from basic post submit button. 
        $('#BasicPostLoadingSpinner').remove();
        $('#SubmitBtn').text(this.postUploadButtonText);
    }

    static IncrementPostCommentCounter(uuid) {
        //Incrememnts the comment counter attribute & changes text for the specified uuid post
        let CommentCounter = $("a[unique-post-id$=" + uuid + "]");
        let count = parseInt(CommentCounter.attr("comment-count"));
        CommentCounter.attr("comment-count", count + 1);
        CommentCounter.text(CommentCounter.text().replace(count, count + 1));
    }

    
    static DecrementPostCommentCounter(uuid) {
        //Incrememnts the comment counter attribute & changes text for the specified uuid post
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
            ],
            disableResizeEditor: true,
            placeholder: 'Type Post Summary Here.......',
            height: 200,

            hint: {
                match: /\B@(\w*)$/,
                search: function (keyword, callback) {
                    if (!keyword || keyword.trim().length === 0) {
                        callback([]);
                        return;
                    }
                    $.ajax({
                        url: `/api/search/${keyword}`,
                        success: function (data) {
                            const results = [];

                            if (data.animePages) {
                               data.animePages.forEach(page => {
                                    results.push({type: "anime", id: page.id, title: page.animeName, url: `/Anime/AnimePageDetails/${page.id}`, img: page.animeImageUrl });
                               }); 
                            }
                            
                            if (data.gamePages) {
                               data.gamePages.forEach(page => {
                                    results.push({type: "game", id: page.id, title: page.name, url: `/Game/GamePage/${page.id}`, img: page.imageUrl});
                               }); 

                            }

                            if (data.users) {
                               data.users.forEach(user => {
                                    results.push({type: "user", id: user.id, title: user.displayName, url: `/Users/GetProfiles?userId=${user.id}`, img: "" });
                               }); 

                            }


                            callback(results);
                        },
                        error: function () {
                            callback([]); // fallback on error
                        }
                    });
                },
                template: function (item) {
                    return '@' + item.title;
                },
                content: function (item) {
                    // What gets inserted into the editor
                    return $(`<a image-url="${item.img}" href="${item.url}" >`)
                        .addClass('link-primary')
                        .text('@' + item.title)[0];
                }
            }
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






