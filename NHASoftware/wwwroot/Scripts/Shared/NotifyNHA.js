class SystemNotification {

    static playNotificationAudio() {
        let audio = new Audio("/Audio/ChatReceived.wav");
        audio.volume = 0.1;
        audio.play();
    }

    static createNotification(message, lifespan = 10000) {
        // this.playNotificationAudio();
        $.jGrowl(message, {
            position: "bottom-left",
            life: lifespan,
            theme: "large-notification"
        });
    }

}