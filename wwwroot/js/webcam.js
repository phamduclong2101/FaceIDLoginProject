function setupWebcam(apiUrl, formId) {
    const video = document.querySelector("#webcam");
    const canvas = document.querySelector("#canvas");
    const captureButton = document.querySelector("#capture");
    const form = document.querySelector(`#${formId}`);

    // Bật webcam
    async function startWebcam() {
        try {
            const stream = await navigator.mediaDevices.getUserMedia({ video: true });
            video.srcObject = stream;
        } catch (err) {
            console.error("Error accessing webcam:", err);
        }
    }

    // Chụp ảnh và lưu vào canvas
    captureButton.addEventListener("click", () => {
        const context = canvas.getContext("2d");
        context.drawImage(video, 0, 0, canvas.width, canvas.height);
    });

    // Gửi ảnh lên server khi submit form
    form.addEventListener("submit", async (e) => {
        e.preventDefault();

        const dataUrl = canvas.toDataURL("image/png");
        const blob = await fetch(dataUrl).then((res) => res.blob());

        const formData = new FormData(form);
        formData.append("faceImage", blob, "webcam_image.png");

        fetch(apiUrl, {
            method: "POST",
            body: formData
        })
        .then((response) => response.json())
        .then((data) => alert(data.message || "Success"))
        .catch((err) => console.error("Error:", err));
    });

    startWebcam();
}

