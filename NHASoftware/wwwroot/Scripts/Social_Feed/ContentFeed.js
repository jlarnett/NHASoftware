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

    $("#SubmitBtn").click(function(e) {
        var userId = RetrieveCurrentUserId();
        var postContent = $($("#MainPostTextbox").summernote("code")).text()

        var newPostObject = {};
        newPostObject.Summary = postContent;
        newPostObject.UserId = userId;
        newPostObject.CreationDate = null;
        newPostObject.ParentPostId = null;

        $.ajax({
            url: '/api/Posts',
            method: 'POST',
            contentType: "application/json; charset=utf-8",
            datatype: 'json',
            data: JSON.stringify(newPostObject),
            headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
            success: function(data) {
                if (data.success) {
                    //Clear summernote textbox after successful submission.
                    $("#MainPostTextbox").summernote('reset');
                    console.log("Successfully submitted post to DB.");
                    $("#MainPostTextboxValidationMessage").hide("slow");
                }
            },
            error: function (data) {
                $("#MainPostTextboxValidationMessage").show("slow");
            }
            
        });
    });

    $("#ContentFeed").on("click", ".comment-send-btn", function (e) { 

        var SendButton = $(e.target);

        var uniquePostIdentifier = SendButton.attr("unique-identifier");
        var commentTextbox = $("div[unique-comment-identifier$="+ uniquePostIdentifier +"]");

        var userId = RetrieveCurrentUserId();
        var fatherPostId = commentTextbox.attr("post-id");
        var postContent = $($("div[unique-comment-identifier$="+ uniquePostIdentifier +"]").summernote("code")).text()

        var newPostObject = {};
        newPostObject.Summary = postContent;
        newPostObject.UserId = userId;
        newPostObject.ParentPostId = fatherPostId;
        newPostObject.CreationDate = null;

        $.ajax({
            url: '/api/Posts',
            method: 'POST',
            contentType: "application/json; charset=utf-8",
            datatype: 'json',
            data: JSON.stringify(newPostObject),
            headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
            success: function(data) {
                if (data.success) {
                    ////Clear summernote textbox after successful submission.
                    $(commentTextbox).summernote('reset');
                    $("span[unique-error-identifier$="+ uniquePostIdentifier +"]").hide("slow");
                    AddCommentDynamically(uniquePostIdentifier, data.data);
                    console.log("Successfully submitted comment to DB.");
                }
            },
            error: function (data) {
                $("span[unique-error-identifier$="+ uniquePostIdentifier +"]").show("slow");
                console.log(data);
            }
        });
    });

    $('#HomeContentFeed').on('scroll', function() {
        //Checks the home pages content feed scrollbar. Whenever scrollbar is bottom position retrieve more posts & load
        //into the home page feed.
        if ($(this).scrollTop() + $(this).innerHeight() >= $(this)[0].scrollHeight) {
            console.log("Loading more post.......................");
            OptimizedMainContentFeedLoad();
        }
    });

    var profileUserId = $('#ContentFeed').attr("profile-user-id");
    if (profileUserId !== undefined) {
        console.log("Attempting to load user profile post.");
        LoadFeedWithProfilePost(profileUserId);
    }

    RebuildFeedTextboxes();
});

function RemovePostFromContentFeed(postId) {
    //Removes the post from the content feed'Ss HTML. This doesn't affect the BD just HTML. 
    var postContainer = $('[post-delete-id=' + postId + ']');
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
        return $.get("/api/posts/GetSocialPostForUserId/" + userId, function(data) {
    });
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
    RetrieveMorePosts().then(function (posts) {
        var feedHtml = [];

        for (var i = 0; i < posts.length; i++) {
            feedHtml.push(GeneratePostRedesign(posts[i]));
        }

        var builtFeedHtml = feedHtml.join("");
        $("#ContentFeed").append(builtFeedHtml);
        RebuildFeedTextboxes();
    });
}


function LoadFeedWithProfilePost(userId) {
    //Loads the id #ContentFeed with all posts created by user. Calls Post WebAPI
    RetrieveAllPostForUser(userId).then(function (posts) {
        var feedHtml = [];

        for (var i = 0; i < posts.length; i++) {
            feedHtml.push(GeneratePostRedesign(posts[i]));
        }

        var builtFeedHtml = feedHtml.join("");
        $("#ContentFeed").empty();
        $("#ContentFeed").append(builtFeedHtml);
        RebuildFeedTextboxes();
    });
}

function GeneratePostRedesign(post) {
    //This method receives post objects & generates the standardized post's html strings
    let uuid = self.crypto.randomUUID();
    var postHtml = [];

    var currentDate = spacetime.now('Africa/Abidjan');
    console.log("Post Id - " + post.id + " Current Date & Time - " + currentDate.month() + "/" + currentDate.date() + "/" + currentDate.year() + " - Time - " + currentDate.hour() + ":" + currentDate.minute() + ":" + currentDate.second() + ":" + currentDate.millisecond());
    var postCreationDate = spacetime(post.creationDate, 'Africa/Abidjan');
    console.log("Post Id - " + post.id + " Post Creation Date & Time - " + postCreationDate.month() + "/" + postCreationDate.date() + "/" + postCreationDate.year() + " - Time - " + postCreationDate.hour() + ":" + postCreationDate.minute() + ":" + postCreationDate.second() + ":" + postCreationDate.millisecond());

    var postDateDifferenceInSeconds = postCreationDate.diff(currentDate, 'second');
    var testDifference = currentDate.diff(postCreationDate, 'second');

    console.log("Test New Diff Method - " + testDifference);
    console.log("Post Date Difference in seconds = " + postDateDifferenceInSeconds);


    postHtml.push('<div class="container-fluid border border-primary mt-2" post-delete-id="', post.id, '">',
            '<div class="row align-items-center">',
                '<!--User Profile image & display name-->',
                '<div class="col-1 px-0">',
                    '<img src="/ProfilePictures/', post.user.profilePicturePath, '" class="img-fluid"/>',
                '</div>',
                '<div class="col-auto border-primary border-start">',
                    '<a class="h3 text-decoration-none profile-link" role="button" userId="', post.user.id, '">', post.user.displayName, '</a>',
                '</div>',
                '<div class="col-auto">',
                    '<a class="h3 text-decoration-none" role="button">-</a>',
                '</div>',
                '<div class="col-2">',
                    '<a class="h3 text-decoration-none" role="button">', GetTimeShortHandString(postDateDifferenceInSeconds), '</a>',
                '</div>',
                '<div class="col-6">',
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
                    InsertCommentTextboxRedesignHtml(post.id, uuid),
            '</div>',
        '</div>');

    return postHtml.join('');
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
            console.log("Post Id - " + data[i].id + " Current Date & Time - " + currentDate.month() + "/" + currentDate.date() + "/" + currentDate.year() + " - Time - " + currentDate.hour() + ":" + currentDate.minute() + ":" + currentDate.second() + ":" + currentDate.millisecond());
            var postCreationDate = spacetime(data[i].creationDate, 'Africa/Abidjan');
            console.log("Post Id - " + data[i].id + " Post Creation Date & Time - " + postCreationDate.month() + "/" + postCreationDate.date() + "/" + postCreationDate.year() + " - Time - " + postCreationDate.hour() + ":" + postCreationDate.minute() + ":" + postCreationDate.second() + ":" + postCreationDate.millisecond());
            var postDateDifferenceInSeconds = postCreationDate.diff(currentDate, 'second');
            console.log("Post Date Difference in seconds = " + postDateDifferenceInSeconds);

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
                        '<div class="col-1">',
                            '<a class="text-decoration-none ms-2 h6">', GetTimeShortHandString(postDateDifferenceInSeconds), '</a>',
                        '</div>',
                        '<div class="col-4"></div>',
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
                '<div class="summernote-comments" post-id="', postId, '" unique-comment-identifier="', uuid  ,'">',
                '</div>',
            '</div>',
            '<button class="btn-dark comment-send-btn col-1" unique-identifier="', uuid ,'">Reply</button>',
            '</div>', '<span unique-error-identifier="', uuid, '" style="display: none; color: red">Error Submitting Post.....</span>');
    }
    else {
        Html.push('');
    }

    return Html.join('');
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
        height: 60,
        toolbar: [
        // [groupName, [list of button]]
        ],
        disableResizeEditor: true,

    });
}