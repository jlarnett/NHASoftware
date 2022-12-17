$(document).ready(function () {

    $('#HomeContentFeed').on('scroll', function() {
        //Checks the home pages content feed scrollbar. Whenever scrollbar is bottom position retrieve more posts & load
        //into the home page feed.
        if ($(this).scrollTop() + $(this).innerHeight() >= $(this)[0].scrollHeight) {
            console.log("Reached the end of the algorithm feed! HURRY AND RELOAD MORE POSTS!!");

            //Uses the .then method to handle AJAX async call
            RetrieveMorePosts().then(function (data) {
                LoadFeedWithNewPosts(data);
            });
        }
    });
});

///AJAX CALL TO api/posts webapi. Returns async result of posts
function RetrieveMorePosts() {
    return $.get("api/posts", function (data) {
    });
}

//Takes posts input data and builds the html for post
function LoadFeedWithNewPosts(posts) {
    console.log("Trying to load new post.....");
    //console.log(posts);

    var postHtml = [];

    for (var i = 0; i < posts.length; i++) {

        let uuid = self.crypto.randomUUID();

        postHtml.push('<div class="MainFeedPostContainer">',
                        '<div>',
                            '<div class="Main-Post-User-Profile">',
                                '<img src="ProfilePictures/', posts[i].user.profilePicturePath, '"class="Main-Post-User-Profile-Picture">',
                            '</div>',
                            '<div class="Main-Post">',
                                '<div class="Main-Post-Top-Header Main-Post-Section">',
                                    '<a class="link-primary feed-profile-link Post-Header-Item" userId="', posts[i].user.id, '">', posts[i].user.displayName, '</a>',
                                    '<a class="link-primary Post-Header-Item">-</a>',
                                    '<a class="link-primary Post-Header-Item">2h</a>',
                                '</div>',
                                '<div class="Main-Post-Summary Main-Post-Section">',
                                    '<p>', posts[i].summary, '</p>',
                                '</div>',
                                '<div class="Main-Post-Bottom-Actionbar Main-Post-Section">',
                                    '<div class="Post-Like-Group">',
                                        '<div class="Like-Group-Item">250</div>',
                                        '<img class="Post-Like-Icon" src="Images/Facebook-Like-Filled.png" />',
                                    '</div>',
                                    '<div class="Post-Like-Group">',
                                        '<div class="Like-Group-Item">250</div>',
                                        '<img class="Post-Like-Icon" src="Images/dislike-Filled.png" />',
                                    '</div>',
                                '</div>',
                                '<div class="Main-Post-Comments Main-Post-Section">',
                                    '<div class="comments-header-row">',
                                        '<a class="link-light hide-comments" unique-post-id="', uuid,'" post-id="', posts[i].id,'">Show Comments</a>',
                                    '</div>',
                                    '<div>',
                                        '<div data-post-id="', posts[i].id, '" unique-post-comment-section-id="', uuid, '" style="display: none;">', 
                                            '<ul>',
                                                '<li>',
                                                    '<div class="comment-container">',
                                                        '<div class="comment-profile-picture">',
                                                            '<img class="comment-profile-picture-item" src="ProfilePictures/DefaultProfilePicture.png" />',
                                                        '</div>',
                                                        '<div class="comment-details">',
                                                            '<h5>Jake Mathers!</h5>',
                                                            'JOHNATHAN THE HATER BATER RATER TATER JOHNATHAN THE HATER BATER RATER TATERJOHNATHAN THE HATER BATER RATER TATERJOHNATHAN THE HATER BATER RATER TATERJOHNATHAN THE HATER BATER RATER TATERJOHNATHAN THE HATER BATER RATER TATER',
                                                            '<div class="Main-Post-Bottom-Actionbar Main-Post-Section">',
                                                                '<div class="Post-Like-Group">',
                                                                    '<div class="Like-Group-Item">250</div>',
                                                                    '<img class="Post-Like-Icon" src="Images/Facebook-Like-Filled.png" />',
                                                                '</div>',
                                                                '<div class="Post-Like-Group">',
                                                                    '<div class="Like-Group-Item">250</div>',
                                                                    '<img class="Post-Like-Icon" src="Images/dislike-Filled.png" />',
                                                                '</div>',
                                                        '</div>',
                                                        '</div>',
                                                    '</div',
                                                '</li>',
                                            '</ul>', 
                                            TryInsertPostCommentTextbox(posts[i].id),
                                        '</div>',
                                    '</div>',
                                '</div>',
                            '</div>',
                        '</div>',
                        '<br />',
                    '</div>');
    }

    function TryInsertPostCommentTextbox(postId) {
        var Html = [];

        var isLoggedIn = CheckUserLoggedIn();
        console.log(isLoggedIn);

        if (isLoggedIn === 'True') {
            Html.push('<div class="comment-textbox-area">',
                '<div class="summernote-comments" post-id="', postId, '">',
                '</div>',
                '<button class="btn-dark comment-send-btn">Reply</button>',
                '</div>');
        }
        else {
            Html.push('');
        }

        return Html.join('');
    }

    var builtPostHtml = postHtml.join("");
    $("#HomeContentFeed").append(builtPostHtml);
    RebuildFeedTextboxes();
 }

function CheckUserLoggedIn() {
    return $("#HomeContentFeed").attr("user-session-active");
}

