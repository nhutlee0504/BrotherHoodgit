async function analyzeText(text) {
    const apiKey = "AIzaSyAgNPZpXYkrNT1dowBC0JM5wFHPoKxPCfo"; // Thay đổi key ở đây nếu cần
    const url = `https://language.googleapis.com/v1/documents:analyzeSentiment?key=${apiKey}`;

    const requestBody = {
        document: {
            type: "PLAIN_TEXT",
            content: text
        }
    };

    try {
        // Gửi yêu cầu đến Google Cloud API để phân tích cảm xúc
        const response = await fetch(url, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(requestBody)
        });

        const result = await response.json();

        // Kiểm tra xem có trả về lỗi không
        if (result.error) {
            return { error: `Lỗi: ${result.error.message}` };
        }

        // Danh sách các từ tục tĩu
        const inappropriateWords = ["mày", "đụ", "vãi", "cặc", "lồn"]; // Thêm hoặc sửa từ tục tĩu ở đây

        // Kiểm tra xem văn bản có chứa từ tục tĩu không
        for (let word of inappropriateWords) {
            if (text.toLowerCase().includes(word.toLowerCase())) {
                return { error: `Văn bản chứa ngôn từ không phù hợp: ${word}` };
            }
        }
        const sentimentScore = result.documentSentiment.score;
        if (sentimentScore < -0.5) { // Có thể điều chỉnh mức độ tiêu cực này theo yêu cầu
            return { error: "Văn bản có cảm xúc tiêu cực." };
        }
        return { message: "" };

    } catch (error) {
        return { error: `Có lỗi xảy ra khi phân tích văn bản: ${error.message}` };
    }
}
