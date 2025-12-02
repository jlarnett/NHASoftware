class SystemNotification {

    static playNotificationAudio() {
        let audio = new Audio("/Audio/ChatReceived.wav");
        audio.volume = 0.1;
        audio.play();
    }

    static createNotification(message) {
        // this.playNotificationAudio();
        $.jGrowl(message, {
            position: "bottom-left",
            life: 10000,
            theme: "large-notification"
        });
    }

}