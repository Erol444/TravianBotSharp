import axios from "axios";

const checkUrlDiscordWebhook = async (url) => {
	try {
		await axios.get(url);
		return true;
	} catch (err) {
		return false;
	}
};

export { checkUrlDiscordWebhook };
