// Đảm bảo hàm này được gán vào đối tượng window
window.startFaceDetection = async function () {
    if (typeof faceapi === 'undefined') {
        console.error("face-api.js not loaded correctly.");
        return;
    }
    await faceapi.nets.ssdMobilenetv1.loadFromUri('/models');
    await faceapi.nets.faceLandmark68Net.loadFromUri('/models');
    await faceapi.nets.faceRecognitionNet.loadFromUri('/models');

    // Khởi động video
    startVideo();
};

async function startVideo() {
    const video = document.getElementById('video');
    const stream = await navigator.mediaDevices.getUserMedia({ video: {} });
    video.srcObject = stream;

    // Khi video bắt đầu phát, gọi hàm nhận diện khuôn mặt
    video.onplaying = () => {
        detectFace(video);
    };
}

async function detectFace(videoElement) {
    const canvas = faceapi.createCanvasFromMedia(videoElement);
    document.body.append(canvas); // Tạo canvas để vẽ kết quả

    const displaySize = { width: videoElement.width, height: videoElement.height };
    faceapi.matchDimensions(canvas, displaySize);

    setInterval(async () => {
        const detections = await faceapi.detectAllFaces(videoElement)
            .withFaceLandmarks()
            .withFaceDescriptors();

        // Vẽ các kết quả nhận diện lên canvas
        const resizedDetections = faceapi.resizeResults(detections, displaySize);
        canvas?.clear();
        faceapi.draw.drawDetections(canvas, resizedDetections);
        faceapi.draw.drawFaceLandmarks(canvas, resizedDetections);
    }, 100); // Cập nhật mỗi 100ms
}
