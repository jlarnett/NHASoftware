class ImageLoader {

    static ShouldPostLoadImagesFromDB() {
        //This function is called every time the user scrolls. Goes over each post & checks whether
        //Images should be loaded. If images need to be loaded then it calls the API Retrieval method
        //and appends it to the post. 
        $('.post-container').filter(function() {
            var postIsPartiallyVisibleInView = Utils.isElementInView(this, false);
            var postIsFullyVisibleInView = Utils.isElementInView(this, true);
        
            var postHasMediaAttached = $(this).attr("media-attached");
            var postHasLoadedMediaAlready = $(this).attr("media-loaded");
            var postId = $(this).attr("post-id");
            var uuid = $(this).attr("post-uuid");

            if (postIsPartiallyVisibleInView && postHasMediaAttached !== "False" && postHasLoadedMediaAlready === "false") {
                $(this).attr("media-loaded", "true");
                ImageLoader.RetrieveImagesForPost(postId, uuid);
            }
        });
    }

    static StoredPostImages = {};
;
    static RetrieveImagesForPost(postId, uuid) {
        //Retrieves images for the specified postId & appends it to the post. Also checks the cache
        //For the chance the post is getting repeated by the post selection algorithm

        ContentFeedUtility.AddSpinnerToImageSection(uuid);

        if (!(postId in this.StoredPostImages)) {
            ContentFeedAjaxCalls.RetrieveImagesForPost(postId).then(function (mediaItems) {
                ContentFeedUtility.RemoveSpinnerFromImageSection(uuid);
                ImageLoader.LoadImagesToPost(mediaItems, uuid);
                ImageLoader.StoredPostImages[postId] = mediaItems;
            });
        }
        else {
            ImageLoader.LoadImagesToPost(this.StoredPostImages[postId], uuid);
            ContentFeedUtility.RemoveSpinnerFromImageSection(uuid);
        }
    }

    static LoadImagesToPost(mediaItems, uuid) {
        //Takes in the list of images retrieved from API Call and creates the HTML for images.
        //The HTML is appended to image section of post. 

        let imageItems = mediaItems.filter((mediaItem) => !mediaItem.isVideo);
        let videoItems = mediaItems.filter((mediaItem) => mediaItem.isVideo);

        if (imageItems.length > 0) {
            var imageHtml = this.GeneratePostImagesHtmlRedesign(imageItems);
            var indicatorHtml = this.GenerateCarouselIndicatorHtml(imageItems, uuid);

            $("#Image-Carousel-Inner-" + uuid).append(imageHtml);
            $("#Image-Carousel-" + uuid).prepend(indicatorHtml);
            ContentFeedUtility.ShowPostImageCarousel(uuid);

            var myCarousel = document.querySelector("#Image-Carousel-" + uuid);
            var carousel = new bootstrap.Carousel(myCarousel, {
                interval: 3000,
            })
        }

        if (videoItems.length > 0) {
            var videoHtml = this.GeneratePostVideosHtml(videoItems);
            $("#Post-Videos-" + uuid).append(videoHtml).show();
        }
    }

    static GenerateCarouselIndicatorHtml(images, uuid) {
        //Returns the HTML for carousel photo indicator - the little dashes that shows which picture & how many in carousel.
        let IndicatorHtml = [];
        let imageCount = 0;

        IndicatorHtml.push('<div id="Image-Carousel-Indicators-', uuid, '" class="carousel-indicators">');

        images.forEach((image) => {
            if (imageCount > 0) {
                IndicatorHtml.push('<button type="button" data-bs-target="#Image-Carousel-', uuid, '" data-bs-slide-to="', imageCount, '" aria-label="Slide ', imageCount + 1, '"></button>');
            }
            else {
                IndicatorHtml.push('<button type="button" data-bs-target="#Image-Carousel-', uuid, '" data-bs-slide-to="', imageCount, '" class="active" aria-current="true" aria-label="Slide 1"></button>');
            }

            imageCount += 1;
        });

        IndicatorHtml.push('</div>');
        return IndicatorHtml.join('');
    }

    static GeneratePostImagesHtmlRedesign(images) {
        //Takes in the list of images retrieved from API Call and creates the HTML for images.
        //The HTML is appended to image section of post. 
        let postImageHtml = [];
        let imageCount = 0;

        images.forEach((image) => {
            var mediaHtml = '<img class="d-block w-100" src="' + image.dataSource + '" alt="Post Image"/>';

            if (imageCount > 0) {
                postImageHtml.push('<div class="carousel-item">', mediaHtml, '</div>');
            }
            else {
                postImageHtml.push('<div class="carousel-item active">', mediaHtml, '</div>');
            }

            imageCount += 1;
        });

        return postImageHtml.join('');
    }

    static GeneratePostVideosHtml(videos) {
        let postVideoHtml = [];

        videos.forEach((video) => {
            let mimeType = this.ResolveVideoMimeType(video.fileExtensionType);
            let videoSource = video.mediaUrl || video.dataSource;
            postVideoHtml.push(
                '<div class="ratio ratio-16x9 rounded-3 overflow-hidden border border-white">',
                '<video class="w-100 h-100 bg-black" controls playsinline preload="metadata">',
                '<source src="', videoSource, '" type="', mimeType, '">',
                'Your browser does not support the video tag.',
                '</video>',
                '</div>'
            );
        });

        return postVideoHtml.join('');
    }

    static ResolveVideoMimeType(fileExtensionType) {
        switch ((fileExtensionType || '').toLowerCase()) {
            case '.mp4':
                return 'video/mp4';
            case '.webm':
                return 'video/webm';
            case '.ogg':
                return 'video/ogg';
            case '.mov':
                return 'video/quicktime';
            default:
                return 'video/mp4';
        }
    }
}