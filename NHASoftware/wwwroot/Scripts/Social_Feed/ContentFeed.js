$(document).ready(function () {

    $('#ContentFeed').on('click', '.delete-post-link' ,function (e) {

        var EventBtn = $(e.target);
        var postId = EventBtn.attr("post-id");
        var isComment = EventBtn.attr("is-comment");

        DeletePost(postId).then(function (data) {
            if (data.success === true) {
                isComment === 'true' ? RemoveCommentFromContentFeed(postId) : RemovePostFromContentFeed(postId);
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
                isComment === 'true' ? RemoveCommentFromContentFeed(postId) : RemovePostFromContentFeed(postId);
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

    //Testing Profile Hide Comments
    $("#ContentFeed").on("click", ".hide-comments", function(e) {
        var EventBtn = $(e.target);
        var postId = EventBtn.attr("post-id");
        var uniquePostId = EventBtn.attr("unique-post-id");

        var commentSectionElement = $("div[unique-comment-section$="+ uniquePostId +"]");

        if (commentSectionElement.is(":visible")) {
            commentSectionElement.hide("slow");
            EventBtn.text("Show Comments")
        }
        else {
            //Fired whenever user clicks on Show Comment link && comment section is not yet visible
            commentSectionElement.show("slow");
            EventBtn.text("Hide Comments")

            //Fires loading of children post HTML
            LoadCommentsRedesign(postId, uniquePostId);
        }
    });

    $("#SubmitBtn").on("click", function(e) {
        e.preventDefault();
        AddSpinnerSubmitPostBtn();
        var form = document.getElementById("BasicPostForm");
        var formData = new FormData(form);
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
                    DynamicallyAddPostToContentFeed(data.data.result);
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
        var form = document.getElementById("CustomPostForm");
        var formData = new FormData(form);
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
                    DynamicallyAddPostToContentFeed(data.data.result);
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

        var SendButton = $(e.target);
        var uniquePostIdentifier = SendButton.attr("unique-identifier");
        var fatherPostId = SendButton.attr("parent-post-id");

        var form = document.querySelector('[comment-form-uuid="' + uniquePostIdentifier + '"]');
        var formData = new FormData(form);
        formData.set("Summary", $($("#comment-textbox-" + uniquePostIdentifier + "").summernote("code")).text());
        formData.set("ParentPostId", fatherPostId);

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
                    $("#comment-textbox-" + uniquePostIdentifier + "").summernote("reset");
                    $("span[unique-error-identifier$="+ uniquePostIdentifier +"]").hide("slow");
                    AddCommentDynamically(uniquePostIdentifier, data.data.result);
                }
            },
            error: function (data) {
                $("span[unique-error-identifier$="+ uniquePostIdentifier +"]").show("slow");
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
    var profileUserId = IsUserProfileFeed();
    if (profileUserId !== undefined) {
        console.log("Attempting to load user profile post.");
        LoadFeedWithProfilePost(profileUserId);
    }

    RebuildFeedTextboxes();
});

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

        if (postIsPartiallyVisibleInView && postHasImagesAttached !== "false" && postHasLoadedImagesAlready === "false") {
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

function RetrieveImagesForPost(postId, uuid) {
    //Retrieves images for the specified postId & appends it to the post. 
    AddSpinnerToImageSection(uuid);
    $.get("/api/posts/GetPostImages/" + postId, function (data) {
        var imagesHtml = GeneratePostImagesHtmlRedesign(data);
        $("[unique-image-section=" + uuid + "]").append(imagesHtml);
        RemoveSpinnerFromImageSection();
    });
}

function RetrieveMorePosts() {
    ///AJAX CALL TO api/posts webapi. Returns async result of posts
    return $.get("api/posts", function (data) {
    });
}

function RetrievePostComments(postId) {
    ///AJAX CALL TO api/posts/findchildrenposts webapi endpoint. 
    //Returns async result all 'Comments' => Comments are just post with fatherPostId populated.
        return $.get("/api/posts/findchildrenposts/" + postId, function(data) {
    });
}

function RetrieveAllPostForUser(userId) {
    //Calls the Post WebAPI & gets all post created under supplied userId
    return $.get("/api/posts/GetSocialPostForUserId/" + userId, function(data) {});
}

function AddCommentDynamically(uuid, data) {
    var commentHtml = [];

    var currentDate = spacetime.now('Africa/Abidjan');
    var postCreationDate = spacetime(data.creationDate);
    var postDateDifferenceInSeconds = postCreationDate.diff(currentDate, 'second');

    commentHtml.push('<li comment-delete-id="', data.id ,'">',
        '<div>',
            '<div class="row align-items-center">',
                '<div class="col-1"></div>',
                '<div class="col-1 px-0">',
                    '<div class="row align">',
                        '<img class="img-fluid col-6 m-auto" src="/ProfilePictures/', data.user.profilePicturePath, '" />', 
                    '</div>',
                '</div>',
                '<div class="col-auto border-primary border-start">',
                    '<a class="profile-link ms-4 h6 text-decoration-none" role="button" userId="', data.user.id, '">', data.user.displayName, '</a>',
                '</div>',
                '<div class="col-auto">',
                    '<a class="text-decoration-none ms-2 h6">', "-", '</a>',
                '</div>',
                '<div class="col-1">',
                    '<a class="text-decoration-none ms-2 h6">', GetTimeShortHandString(postDateDifferenceInSeconds), '</a>',
                '</div>',
                '<div class="col-4"></div>',
                '<div class="col-auto">',
                    GeneratePostActionButton(data),
                '</div>',
            '</div>',
            '<div class="row align-items-center">',
                '<div class="col-2"></div>',
                '<div class="col-6 align-middle ms-4 p-2 border-primary border-bottom">', data.summary, '</div>',
            '</div>',
            GenerateSocialLikeSectionRedesign(data),
            '<hr/>',
        '</div>',
        '</li>');
        
        var commentListElement = $("ul[unique-comment-list$=" + uuid + "]");
        commentListElement.append(commentHtml.join(''));
}

function OptimizedMainContentFeedLoad() {
    //Loads the id #ContentFeed with all posts created by user. Calls Post WebAPI
    AddSpinnerToContentFeed();
    RetrieveMorePosts().then(function (posts) {
        for (var i = 0; i < posts.length; i++) {
            $("#ContentFeed").append(GeneratePostRedesign(posts[i]))
        }

        RebuildFeedTextboxes();
        RemoveSpinnerFromContentFeed();
    });
}


function LoadFeedWithProfilePost(userId) {
    //Loads the id #ContentFeed with all posts created by user. Calls Post WebAPI
    AddSpinnerToContentFeed();
    RetrieveAllPostForUser(userId).then(function (posts) {
        var feedHtml = [];

        for (var i = 0; i < posts.length; i++) {
            feedHtml.push(GeneratePostRedesign(posts[i]));
        }

        var builtFeedHtml = feedHtml.join("");
        $("#ContentFeed").empty();
        $("#ContentFeed").append(builtFeedHtml);
        RebuildFeedTextboxes();
        RemoveSpinnerFromContentFeed();
    });
}

function GeneratePostRedesign(post) {
    //This method receives post objects & generates the standardized post's html strings
    let uuid = self.crypto.randomUUID();
    var postHtml = [];

    //Getting Current Date & Post Date. Calculate difference between the two.
    var currentDate = spacetime.now('Africa/Abidjan');
    var postCreationDate = spacetime(post.creationDate, 'Africa/Abidjan');
    var postDateDifferenceInSeconds = postCreationDate.diff(currentDate, 'second');

    //Date Debugging console logs
    //console.log("Post Id - " + post.id + " Post Creation Date & Time - " + postCreationDate.month() + "/" + postCreationDate.date() + "/" + postCreationDate.year() + " - Time - " + postCreationDate.hour() + ":" + postCreationDate.minute() + ":" + postCreationDate.second() + ":" + postCreationDate.millisecond());
    //console.log("Post Id - " + post.id + " Current Date & Time - " + currentDate.month() + "/" + currentDate.date() + "/" + currentDate.year() + " - Time - " + currentDate.hour() + ":" + currentDate.minute() + ":" + currentDate.second() + ":" + currentDate.millisecond());
    //console.log("Post Date Difference in seconds = " + postDateDifferenceInSeconds);

    postHtml.push('<div class="container-fluid border border-primary mt-2 post-container" post-id="', post.id, '"', 'images-attached="', post.hasImagesAttached, '" images-loaded="false" post-uuid="', uuid, '">',
            '<div class="row align-items-center">',
                '<!--User Profile image & display name-->',
                '<div class="col-1 px-0">',
                    '<img src="/ProfilePictures/', post.user.profilePicturePath, '" class="img-fluid"/>',
                '</div>',
                '<div class="col-sm-auto border-primary border-start">',
                    '<a class="h3 text-decoration-none profile-link" role="button" userId="', post.user.id, '">', post.user.displayName, '</a>',
                '</div>',
                '<div class="col-auto">',
                    '<a class="h3 text-decoration-none" role="button">-</a>',
                '</div>',
                '<div class="col-2">',
                    '<a class="h3 text-decoration-none" role="button">', GetTimeShortHandString(postDateDifferenceInSeconds), '</a>',
                '</div>',
                '<div class="col">',
                '</div>',
                '<!--Post Action dropdown-->',
                '<div class="col-auto">',
                    GeneratePostActionButton(post),
                '</div>',
            '</div>',
            '<div class="row">',
                '<div class="col-sm-1 px-0"></div>',
                '<div class="col border-bottom border-primary">',
                    '<div class="h4 text-primary">Summary</div>',
                '</div>',
            '</div>',
            '<!--Posts summary section. Text content is posted here-->',
            '<div class="row text-break">',
                '<div class="col-sm-1 px-0"></div>',
                '<p class="text-black col text-white border-bottom border-start border-primary">', post.summary, '</p>',
            '</div>',
            '<div class="row text-break">',
                '<div class="col-sm-1 px-0"></div>',
                '<div class="row col px-0" unique-image-section="', uuid, '">',
                '</div>',
            '</div>',
            '<!--Posts Likes & Comment Show section-->',
            '<div class="row align-items-center m-2">',
                '<div class="col-auto">',
                    '<a class="link-light hide-comments" unique-post-id="', uuid, '" post-id="', post.id, '" role="button">Show Comments</a>',
                '</div>',
                '<div class="col-5"></div>',
                GenerateSocialLikeSectionRedesign(post),
            '</div>',
            '<div unique-comment-section="', uuid, '" style="display: none">',
                    '<ul unique-comment-list="', uuid, '" class="list-unstyled">',
                    '</ul>',
                    "<form method='post' enctype='multipart/form-data' comment-form-uuid='", uuid ,"'>",
                        InsertCommentTextboxRedesignHtml(post.id, uuid),
                    "</form>",
            '</div>',
        '</div>');

    return postHtml.join('');
}

function GeneratePostImagesHtmlRedesign(images) {
    //Takes in the list of images retrieved from API Call and creates the HTML for images.
    //The HTML is appended to image section of post. 
    postImageHtml = [];
    images.forEach((image) => postImageHtml.push('<div class="col-5"><img class="col img-thumbnail MainFeedPostImages" src="', image , '"/></div>'));
    return postImageHtml.join('');
}

function GeneratePostActionButton(post) {
    //Generates the post action dropdown button. Available actions are determined & added to the dropdown before returning
    actionButtonHtml = [];

    actionButtonHtml.push('<div class="dropstart">',
                            '<button class="btn btn-dark dropdown-toggle" type="button" id="dropstartMenuButton" data-bs-toggle="dropdown" aria-expanded="false">',
                                '<img src="/Images/options.png" class="img-fluid"/>',
                            '</button>',
                            '<ul class="dropdown-menu" aria-labelledby="dropstartMenuButton" style="">',
                                '<li><h6 class="dropdown-header">Actions</h6></li>',
                                '<li><hr class="dropdown-divider"></li>',
                                GenerateHideActionButtonLink(post),
                                GenerateDeleteActionButtonLink(post),
                                GenerateReportActionButtonLink(post),
                            '</ul>',
                        '</div>');

    return actionButtonHtml.join('');
}

function GenerateDeleteActionButtonLink(post) {
    //Returns a string of the Delete Post Action Button HTML as long as user is admin or the owner of the post. 
    var isCommentBool = IsComment(post);

    if (post.user.id === RetrieveCurrentUserId() || IsCurrentUserAdmin() === "True") {
        return '<li><a class="dropdown-item delete-post-link" post-id="' + post.id +'" is-comment="' + isCommentBool +'">Delete Post</a></li>';
    }
}

function GenerateHideActionButtonLink(post) {
    //Returns a string of the Hide Post Action Button HTML as long as user is admin or the owner of the post. 
    var isCommentBool = IsComment(post);
    if (post.user.id === RetrieveCurrentUserId() || IsCurrentUserAdmin() === "True") {
        return '<li><a class="dropdown-item hide-post-link" post-id="' + post.id + '" is-comment="' + isCommentBool + '">Hide Post</a></li>';
    }
}

function GenerateReportActionButtonLink(post) {
    //Returns a string of the Report Post Action Button HTML as long as user is logged in to site. 
    var isCommentBool = IsComment(post);
    if (CheckUserSessionIsActive() === "True") {
            return '<li><a class="dropdown-item report-post-link" post-id="' + post.id + '" is-comment="' + isCommentBool + '">Report Post</a></li>';
    }
}

function IsComment(post) {
    //Returns bool whether the supplied post is a comment or a primary post. 
    if (post.parentPostId === null) { 
        return false;
    }
    return true;
}

function LoadCommentsRedesign(id, uuid) {
    //Loads comments for post using bootstrap redesign. Takes the post-id & uuid to determine which comment section to load
    //Called by Hide Comments js script. 
    var commentHtml = [];
    var currentDate = spacetime.now('Africa/Abidjan');

    RetrievePostComments(id).then(function (data) {
        for (var i = 0; i < data.length; i++) {
            var postCreationDate = spacetime(data[i].creationDate, 'Africa/Abidjan');
            var postDateDifferenceInSeconds = postCreationDate.diff(currentDate, 'second');

            //Date DEBUG LOGGING
            //console.log("Post Id - " + data[i].id + " Current Date & Time - " + currentDate.month() + "/" + currentDate.date() + "/" + currentDate.year() + " - Time - " + currentDate.hour() + ":" + currentDate.minute() + ":" + currentDate.second() + ":" + currentDate.millisecond());
            //console.log("Post Id - " + data[i].id + " Post Creation Date & Time - " + postCreationDate.month() + "/" + postCreationDate.date() + "/" + postCreationDate.year() + " - Time - " + postCreationDate.hour() + ":" + postCreationDate.minute() + ":" + postCreationDate.second() + ":" + postCreationDate.millisecond());
            //console.log("Post Date Difference in seconds = " + postDateDifferenceInSeconds);

            commentHtml.push('<li comment-delete-id="', data[i].id ,'">',
                '<div>',
                    '<div class="row align-items-center">',
                        '<div class="col-1"></div>',
                        '<div class="col-1 px-0">',
                            '<div class="row align">',
                                '<img class="img-fluid col-6 m-auto" src="/ProfilePictures/', data[i].user.profilePicturePath, '" />', 
                            '</div>',
                        '</div>',
                        '<div class="col-auto border-primary border-start">',
                            '<a class="profile-link ms-4 h6 text-decoration-none" role="button" userId="', data[i].user.id, '">', data[i].user.displayName, '</a>',
                        '</div>',
                        '<div class="col-auto">',
                            '<a class="text-decoration-none ms-2 h6">', "-", '</a>',
                        '</div>',
                        '<div class="col-2">',
                            '<a class="text-decoration-none ms-2 h6">', GetTimeShortHandString(postDateDifferenceInSeconds), '</a>',
                        '</div>',
                        '<div class="col"></div>',
                        '<div class="col-auto">',
                            GeneratePostActionButton(data[i]),
                        '</div>',
                    '</div>',
                    '<div class="row align-items-center">',
                        '<div class="col-2"></div>',
                        '<div class="col-6 align-middle ms-4 p-2 border-primary border-bottom">', data[i].summary, '</div>',
                    '</div>',
                    GenerateSocialLikeSectionRedesign(data[i]),
                    '<hr/>',
                '</div>',
                '</li>');

            var commentListElement = $("ul[unique-comment-list$=" + uuid + "]");

            commentListElement.empty();
            commentListElement.html(commentHtml.join(''));
        }
    });
}
function GenerateSocialLikeSectionRedesign(post) {
    //Takes the user post & generates the like section html. Determines Initial Like Icon states using value returned in postDto object
    var likeSectionHtml = [];

    var likeSrcImage = (post.userLikedPost ? "/images/facebook-like-filled.png" : "/images/facebook-like.png");
    var dislikeSrcImage = (post.userDislikedPost ? "/images/dislike-Filled.png" : "/images/dislike_remake.png");

    likeSectionHtml.push('<div class="row mt-2">',
        '<div class="col-5"></div>',
        '<div class="col-1">',
            '<div class="row">',
                '<div class="col-auto me-2" like-counter-post-id="', post.id, '">', post.likeCount ,'</div>',
                '<div class="col-sm-5">',
                    '<img class="post-like img-fluid" role="button" post-id="', post.id,'" src="', likeSrcImage , '" />',
                '</div>',
            '</div>',
        '</div>',
        '<div class="col-1">',
            '<div class="row">',
                '<div class="col-auto me-2" dislike-counter-post-id="', post.id,'">', post.dislikeCount ,'</div>',
                '<div class="col-sm-5">',
                    '<img class="post-dislike img-fluid" role="button" post-id="', post.id,'" src="', dislikeSrcImage , '" />',
                '</div>',
            '</div>',
        '</div>',
    '</div>');

    return likeSectionHtml.join('');
}

function InsertCommentTextboxRedesignHtml(postId, uuid) {
//Tries to insert textbox below comment section. Checks if user is logged in & returns the textbox html if logged in.
    var Html = [];
    var isLoggedIn = CheckUserSessionIsActive();

    if (isLoggedIn === 'True') {
        Html.push('<div class="row mb-2">',
            '<div class="col-2"></div>',
            '<div class="col-5">',
                '<textarea class="summernote-comments" post-id="', postId, '" id="comment-textbox-', uuid  ,'" name="Summary">',
                '</textarea>',
            '</div>',
            '<button class="btn-dark comment-send-btn col-1" unique-identifier="', uuid ,'" parent-post-id="', postId ,'">Reply</button>',
            '</div>', '<span unique-error-identifier="', uuid, '" style="display: none; color: red">Error Submitting Post.....</span>');
    }
    else {
        Html.push('');
    }

    return Html.join('');
}

function DynamicallyAddPostToContentFeed(post) {
    var postHtml = GeneratePostRedesign(post);
    $("#ContentFeed").prepend(postHtml);
    RebuildFeedTextboxes();
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