$(document).ready(function () {

    $('#ContentFeed').on('click', '.delete-post-link' ,function (e) {

        var EventBtn = $(e.target);
        var postId = EventBtn.attr("post-id");
        var isComment = EventBtn.attr("is-comment");

        DeletePost(postId).then(function (data) {
            if (data.success === true) {
                isComment === "True" ? RemoveCommentFromContentFeed(postId) : RemovePostFromContentFeed(postId);
            }
            else {
                console.log("Failed to send DELETE Post request to api. ");
            }
        });
    });

    $('#ContentFeed').on('click', '.hide-post-link' ,function (e) {

        var EventBtn = $(e.target);
        var postId = EventBtn.attr("post-id");
        var isComment = EventBtn.attr("is-comment");

        HidePostFromProfile(postId).then(function (data) {
            if (data.success === true) {
                isComment === 'True' ? RemoveCommentFromContentFeed(postId) : RemovePostFromContentFeed(postId);
            }
            else {
                console.log("Failed to send Hide Post request to api. ");
            }
        });
    });

    $('#ContentFeed').on('click', '.report-post-link' ,function (e) {

    });

    $("#ContentFeed").on('click', '.profile-link', function (e) {
        var SendButton = $(e.target);
        var userId = SendButton.attr("userId");
        var profileUrl = "/Users/GetProfiles?userId=" + userId;
        UserNavigatedToLink(profileUrl);
    });

    $("#ContentFeed").on('click', '.profile-link', function (e) {
        var SendButton = $(e.target);
        var userId = SendButton.attr("userId");
        var profileUrl = "/Users/GetProfiles?userId=" + userId;
        UserNavigatedToLink(profileUrl);
    });

    var loadedPostsComments = [];

    //Testing Profile Hide Comments
    $("#ContentFeed").on("click", ".hide-comments", function(e) {
        var EventBtn = $(e.target);
        var postId = EventBtn.attr("post-id");
        var uniquePostId = EventBtn.attr("unique-post-id");
        var commentCount = EventBtn.attr("comment-count");

        var commentSectionElement = $("div[unique-comment-section$="+ uniquePostId +"]");

        if (commentSectionElement.is(":visible")) {
            commentSectionElement.hide(250);
            EventBtn.text("Show Comments (" + commentCount + ")")
        }
        else {
            //Fired whenever user clicks on Show Comment link && comment section is not yet visible
            commentSectionElement.show(250);
            EventBtn.text("Hide Comments (" + commentCount + ")")

            if (!containsObject(uniquePostId, loadedPostsComments)) {
                LoadCommentsRedesign(postId, uniquePostId);
                loadedPostsComments.push(uniquePostId);
            }
        }
    });

    $("#SubmitBtn").on("click", function(e) {
        e.preventDefault();
        AddSpinnerSubmitPostBtn();

        let form = document.getElementById("BasicPostForm");
        let formData = new FormData(form);
        formData.set("Summary", $($("#MainPostTextbox").summernote("code")).text())

        $.ajax({
            url: '/api/Posts/BasicPost',
            method: 'POST',
            contentType: false,
            processData: false,
            data: formData,
            headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
            success: function(data) {
                if (data.success) {
                    //Clear summernote textbox after successful submission.
                    RemoveSpinnerSubmitPostBtn();
                    ClearBasicPostAfterSuccessfulAPIResponse();
                    TryToDynamicallyAddPostToContentFeed(data.post);
                }
            },
            error: function (data) {
                RemoveSpinnerSubmitPostBtn();
                $("#MainPostTextboxValidationMessage").show("slow");
            }
        });
    });

   $("#SubmitCustomPostBtn").on("click", function(e) {
        e.preventDefault();
        AddSpinnerSubmitCustomPostBtn();

        let form = document.getElementById("CustomPostForm");
        let formData = new FormData(form);
        formData.set("Summary", $($("#CustomPostTextbox").summernote("code")).text())

        $.ajax({
            url: '/api/Posts/CustomizedPost',
            method: 'POST',
            contentType: false,
            processData: false,
            data: formData,
            headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
            success: function(data) {
                if (data.success) {
                    //Clear custom post & dynamically add post to content feed
                    RemoveSpinnerSubmitCustomPostBtn();
                    ClearCustomPostAfterSuccessfulAPIResponse();
                    TryToDynamicallyAddPostToContentFeed(data.post);
                    HideCustomPostModal();
                }
            },
            error: function (data) {
                RemoveSpinnerSubmitCustomPostBtn();
                $("#CustomPostValidationMessage").show("slow");
            }
        });
    });

    $("#ContentFeed").on("click", ".comment-send-btn", function (e) { 
        e.preventDefault();

        let SendButton = $(e.target);
        let uuid = SendButton.attr("unique-identifier");
        let parentPostId = SendButton.attr("parent-post-id");

        let form = document.querySelector('[comment-form-uuid="' + uuid + '"]');
        let formData = new FormData(form);

        let commentTextboxJqueryLocator = '[comment-textbox-uuid="' + uuid + '"]'
        let commentText  = $(commentTextboxJqueryLocator).summernote('code').replace(/<\/p>/gi, "\n")
                .replace(/<br\/?>/gi, "\n")
                .replace(/<\/?[^>]+(>|$)/g, "")
                .replace(/&nbsp;|<br>/g, ' ');

        formData.set("ParentPostId", parentPostId);
        formData.set("Summary", commentText);

        $.ajax({
            url: '/api/Posts/BasicPost',
            method: 'POST',
            contentType: false,
            processData: false,
            data: formData,
            headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
            success: function(data) {
                if (data.success) {
                    ////Clear summernote textbox after successful submission.
                    ClearCommentTextboxAfterSuccessfulAPIResponse(uuid);
                    TryToAddCommentDynamically(uuid, data.post);
                }
            },
            error: function (data) {
                $("span[unique-error-identifier$="+ uuid +"]").show("slow");
                console.log(data);
            }
        });
    });

    $(window).on("scroll", function() {
        //Called whenever the user scrolls the document. Handles loading more post for infinite feed loop
        //Handles loading images as the user scrolls the feed. This keeps the base load time for post faster
        if (IsUserProfileFeed() === undefined) {
            ShouldContentFeedShouldLoadMorePosts();
        }

        ShouldPostLoadImagesFromDB();
    });


    //Tries to load content feed with user profile post only if content feed is on user profile page.
    let profileUserId = IsUserProfileFeed();
    if (profileUserId !== undefined) {
        console.log("Attempting to load user profile post.");
        LoadFeedWithProfilePost(profileUserId);
    }

    RebuildFeedTextboxes();
});

function containsObject(obj, list) {
    var i;
    for (i = 0; i < list.length; i++) {
        if (list[i] === obj) {
            return true;
        }
    }

    return false;
}

function IsUserProfileFeed() {
    //Returns profile-user-Id This determines whether the content feed is main feed or user profile
    return $('#ContentFeed').attr("profile-user-id");
}

function ShouldPostLoadImagesFromDB() {
    //This function is called every time the user scrolls. Goes over each post & checks whether
    //Images should be loaded. If images need to be loaded then it calls the API Retrieval method
    //and appends it to the post. 
    $('.post-container').filter(function() {
        var postIsPartiallyVisibleInView = Utils.isElementInView(this, false);
        var postIsFullyVisibleInView = Utils.isElementInView(this, true);
        
        var postHasImagesAttached = $(this).attr("images-attached");
        var postHasLoadedImagesAlready = $(this).attr("images-loaded");
        var postId = $(this).attr("post-id");
        var uuid = $(this).attr("post-uuid");

        if (postIsPartiallyVisibleInView && postHasImagesAttached !== "False" && postHasLoadedImagesAlready === "false") {
            $(this).attr("images-loaded", "true");
            RetrieveImagesForPost(postId, uuid);
        }
    });
}

function ShouldContentFeedShouldLoadMorePosts() {
    //Checks the home page scroll bar. When the scrollbar is lower than the specified percentage it fires home feed content loading.
    var scrollbarValue = $(window).scrollTop() + $(window).height();
    var windowHeightPercentToLoadFeed = 55;
    var windowHeight = Math.trunc($(document).height());
    var percentScrolled = Math.trunc((scrollbarValue / windowHeight) * 100);

    //Debugging Log
    //console.log("Percent Scrolled - " + percentScrolled);

    if(percentScrolled >= windowHeightPercentToLoadFeed || percentScrolled == 100) {
        LoadMorePostToMainContentFeed();
    }
}

var canGo = true;
function LoadMorePostToMainContentFeed() {
    //Calls the Optimized feed load. Called whenever the scroll bar event conditionals fire off.
    //Contains a delay timer to stop the content from loading multiple times instantly.

    delay = 5000; // 5 seconds

    if (canGo) {
        if (canGo) {
            canGo = false;
            console.log("Loading More Post!!!!!");
            OptimizedMainContentFeedLoad();
            setTimeout(function () {canGo = true;}, delay)
        } 
    }
}

function RemovePostFromContentFeed(postId) {
    //Removes the post from the content feed'Ss HTML. This doesn't affect the BD just HTML. 
    var postContainer = $('[post-id=' + postId + ']');
    postContainer.remove();
}

function RemoveCommentFromContentFeed(postId) {
    //Removes comment from the content feed's HTML. This doesn't affect the DB just HTML.
    var commentContainer = $('[comment-delete-id=' + postId + ']');
    commentContainer.remove();
}

function DeletePost(postId) {
    //sends DELETE request to post API. Sets the isDeletedFlag to true in DB. 
    var apiURL = '/api/posts/' + postId;

    return $.ajax({
        url: apiURL,
        method: 'DELETE',
        contentType: "application/json; charset=utf-8",
        datatype: 'json',
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() }
    });
}

function HidePostFromProfile(postId) {
    //sends DELETE request to post API. Sets the isHiddenFromUserProfile flag in DB to true. 
    var apiURL = '/api/posts/hide/' + postId;

    return $.ajax({
        url: apiURL,
        method: 'DELETE',
        contentType: "application/json; charset=utf-8",
        datatype: 'json',
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() }
    });
}



function RetrieveMorePosts() {
    ///AJAX CALL TO api/posts webapi. Returns async result of posts
    return $.get("/Home/ReturnSocialPosts", function (data) {
    });
}

function RetrievePostComments(postId) {
    ///AJAX CALL TO api/posts/findchildrenposts webapi endpoint. 
    //Returns async result all 'Comments' => Comments are just post with fatherPostId populated.
        return $.get("/Home/ReturnCommentPosts/" + postId, function(data) {
    });
}

function RetrieveAllPostForUser(userId) {
    //Calls the Post WebAPI & gets all post created under supplied userId
    return $.get("/GetAllPostForUser/" + userId, function(data) {});
}



function OptimizedMainContentFeedLoad() {
    //Loads the id #ContentFeed with all posts created by user. Calls Home Base Controller (Simplifies Partial View Return)
    AddSpinnerToContentFeed();
    RetrieveMorePosts().then(function (posts) {
        $("#ContentFeed").append(posts)
        RebuildFeedTextboxes();
        RemoveSpinnerFromContentFeed();
    });
}


function LoadFeedWithProfilePost(userId) {
    //Loads the id #ContentFeed with all posts created by user. Calls Post WebAPI
    AddSpinnerToContentFeed();
    RetrieveAllPostForUser(userId).then(function (posts) {
        $("#ContentFeed").empty();
        $("#ContentFeed").append(posts);
        RebuildFeedTextboxes();
        RemoveSpinnerFromContentFeed();
    });
}

function RetrieveImagesForPost(postId, uuid) {
    //Retrieves images for the specified postId & appends it to the post. 
    AddSpinnerToImageSection(uuid);
    $.get("/api/posts/GetPostImages/" + postId, function (data) {
        var imagesHtml = GeneratePostImagesHtmlRedesign(data);
        $("[unique-image-section=" + uuid + "]").append(imagesHtml);
        RemoveSpinnerFromImageSection();
    });
}

function GeneratePostImagesHtmlRedesign(images) {
    //Takes in the list of images retrieved from API Call and creates the HTML for images.
    //The HTML is appended to image section of post. 
    postImageHtml = [];
    images.forEach((image) => postImageHtml.push('<div class="col-5"><img class="col img-thumbnail MainFeedPostImages" src="', image , '"/></div>'));
    return postImageHtml.join('');
}

function LoadCommentsRedesign(id, uuid) {
    //Loads comments for post using bootstrap redesign. Takes the post-id & uuid to determine which comment section to load
    //Called by Hide Comments js script. 

    RetrievePostComments(id).then(function (data) {
        var commentListElement = $("ul[unique-comment-list$=" + uuid + "]");
        commentListElement.empty();
        commentListElement.html(data);
    });
}

function TryToDynamicallyAddPostToContentFeed(post) {

    $.ajax({
        url: '/ReturnPostPartialView',
        method: 'POST',
        data: post,
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
        success: function(post) {
            $("#ContentFeed").prepend(post);
            RebuildFeedTextboxes();
        },
        error: function (data) {

        }
            
    });
}

function TryToAddCommentDynamically(uuid, commentDto) {

    $.ajax({
        url: '/ReturnCommentPartialView',
        method: 'POST',
        data: commentDto,
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
        success: function(commentPartialView) {
            var commentListElement = $("ul[unique-comment-list$=" + uuid + "]");
            commentListElement.append(commentPartialView);
        },
        error: function (data) {

        }
            
    });
}

function ClearCustomPostAfterSuccessfulAPIResponse() {
    $("#CustomPostTextbox").summernote('reset');
    $("#CustomPostImageFileInput").val(null);
    $("#CustomPostValidationMessage").hide("slow");
}

function ClearBasicPostAfterSuccessfulAPIResponse() {
    $("#MainPostTextbox").summernote('reset');
    $("#MainPostTextboxValidationMessage").hide("slow");
}

function ClearCommentTextboxAfterSuccessfulAPIResponse(uuid) {
    var commentTextboxJqueryLocator = '[comment-textbox-uuid="' + uuid + '"]';
    $(commentTextboxJqueryLocator).summernote("reset");
    $("span[unique-error-identifier$="+ uuid +"]").hide("slow");
}

function HideCustomPostModal() {
    $('#AddPostPhotosModal').modal('hide');
}

var postUploadButtonText = "";

function AddSpinnerSubmitCustomPostBtn() {
    postUploadButtonText = $('#SubmitCustomPostBtn').text();
    $('#SubmitCustomPostBtn').text("");
    $('#SubmitCustomPostBtn').prepend("<span id='LoadingSpinner' class='spinner-border spinner-border' role='status' aria-hidden='true'></span>");
}

function AddSpinnerSubmitPostBtn() {
    postUploadButtonText = $('#SubmitBtn').text();
    $('#SubmitBtn').text("");
    $('#SubmitBtn').prepend("<span id='LoadingSpinner' class='spinner-border spinner-border' role='status' aria-hidden='true'></span>");
}

function RemoveSpinnerSubmitCustomPostBtn() {
    $('#LoadingSpinner').remove();
    $('#SubmitCustomPostBtn').text(postUploadButtonText);
}

function RemoveSpinnerSubmitPostBtn() {
    $('#LoadingSpinner').remove();
    $('#SubmitBtn').text(postUploadButtonText);
}

function AddSpinnerToContentFeed() {
    $("#ContentFeed").append('<div id="LoadingSpinner" class="text-center mt-2" ><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>');
}

function RemoveSpinnerFromContentFeed() {
    $('#LoadingSpinner').remove();
}

function AddSpinnerToImageSection(uuid) {
    $("[unique-image-section=" + uuid + "]").append('<div id="LoadingSpinner" class="text-center mt-2" ><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>');
}

function RemoveSpinnerFromImageSection(uuid) {
    $('#LoadingSpinner').remove();
}

function RebuildFeedTextboxes() {
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
        // [groupName, [list of button]]
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