import { initializeApp } from "https://www.gstatic.com/firebasejs/11.0.2/firebase-app.js";
import { getStorage, ref, uploadBytes, getDownloadURL } from "https://www.gstatic.com/firebasejs/11.0.2/firebase-storage.js";

const firebaseConfig = {
    apiKey: "AIzaSyBELVlqN9FdTMfnVY1cUr9mghf2UtxmrOA",
    authDomain: "dbbrotherhood-ac2f1.firebaseapp.com",
    projectId: "dbbrotherhood-ac2f1",
    storageBucket: "dbbrotherhood-ac2f1.appspot.com",
    messagingSenderId: "1081154085696",
    appId: "1:1081154085696:web:6393042ba900d59fdfd0a5",
    measurementId: "G-CVQDJ0LX4S"
};

const app = initializeApp(firebaseConfig);
const storage = getStorage(app);

window.uploadImageToFirebase = async function (fileObject) {
    const [fileName, base64String] = fileObject;

    // Chuyển đổi Base64 thành Blob
    const byteCharacters = atob(base64String);
    const byteArrays = [];
    for (let offset = 0; offset < byteCharacters.length; offset += 512) {
        const slice = byteCharacters.slice(offset, offset + 512);
        const byteNumbers = new Array(slice.length);
        for (let i = 0; i < slice.length; i++) {
            byteNumbers[i] = slice.charCodeAt(i);
        }
        const byteArray = new Uint8Array(byteNumbers);
        byteArrays.push(byteArray);
    }
    const fileBlob = new Blob(byteArrays, { type: "image/jpeg" });

    // Thực hiện upload
    const storageRef = ref(storage, `ImageTest/${fileName}`);
    try {
        const snapshot = await uploadBytes(storageRef, fileBlob);
        const downloadUrl = await getDownloadURL(snapshot.ref);
        return downloadUrl; // Trả về URL của ảnh
    } catch (error) {
        console.error("Upload failed", error);
        throw error;
    }
};
