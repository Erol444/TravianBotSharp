import axios from "axios";

const checkUrlDiscordWebhook = async (url) => {
	try {
		const { status } = await axios.get(url);
		return status;
	} catch (err) {
		return err.response.status;
	}
};

export { checkUrlDiscordWebhook };
