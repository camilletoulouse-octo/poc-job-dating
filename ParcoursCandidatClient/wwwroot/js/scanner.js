let _scanLoop = null;

window.scanner = {
    /**
     * Ouvre la caméra arrière du téléphone dans un élément <video>.
     * Retourne true si le flux a démarré, false sinon.
     * @param {string} videoElementId - L'id de l'élément <video> cible.
     * @returns {Promise<boolean>}
     */
    ouvrirCamera: async function (videoElementId) {
        const video = document.getElementById(videoElementId);
        if (!video) return false;

        try {
            const stream = await navigator.mediaDevices.getUserMedia({
                video: { facingMode: "environment" },
                audio: false
            });
            video.srcObject = stream;
            await video.play();
            return true;
        } catch (err) {
            console.error("Erreur lors de l'ouverture de la caméra :", err);
            return false;
        }
    },

    /**
     * Arrête tous les flux vidéo actifs sur un élément <video> et stoppe la boucle de scan.
     * @param {string} videoElementId - L'id de l'élément <video> cible.
     */
    fermerCamera: function (videoElementId) {
        if (_scanLoop !== null) {
            cancelAnimationFrame(_scanLoop);
            _scanLoop = null;
        }

        const video = document.getElementById(videoElementId);
        if (!video || !video.srcObject) return;

        const tracks = video.srcObject.getTracks();
        tracks.forEach(track => track.stop());
        video.srcObject = null;
    },

    /**
     * Vérifie si le navigateur supporte l'accès à la caméra.
     * @returns {boolean}
     */
    estCameraDisponible: function () {
        return !!(navigator.mediaDevices && navigator.mediaDevices.getUserMedia);
    },

    /**
     * Démarre la boucle de décodage QR code sur le flux vidéo.
     * Appelle dotNetRef.invokeMethodAsync("OnQrCodeDetecte", data) dès qu'un QR code est détecté.
     * @param {string} videoElementId - L'id de l'élément <video> source.
     * @param {string} canvasElementId - L'id de l'élément <canvas> utilisé pour capturer les frames.
     * @param {DotNetObjectReference} dotNetRef - Référence .NET pour le callback.
     */
    demarrerScan: function (videoElementId, canvasElementId, dotNetRef) {
        const video = document.getElementById(videoElementId);
        const canvas = document.getElementById(canvasElementId);
        if (!video || !canvas) return;

        const ctx = canvas.getContext("2d", { willReadFrequently: true });

        function tick() {
            if (video.readyState === video.HAVE_ENOUGH_DATA) {
                canvas.width = video.videoWidth;
                canvas.height = video.videoHeight;
                ctx.drawImage(video, 0, 0, canvas.width, canvas.height);

                const imageData = ctx.getImageData(0, 0, canvas.width, canvas.height);
                const code = jsQR(imageData.data, imageData.width, imageData.height, {
                    inversionAttempts: "dontInvert"
                });

                if (code) {
                    dotNetRef.invokeMethodAsync("OnQrCodeDetecte", code.data);
                    return; // Arrête la boucle après détection
                }
            }
            _scanLoop = requestAnimationFrame(tick);
        }

        _scanLoop = requestAnimationFrame(tick);
    }
};
