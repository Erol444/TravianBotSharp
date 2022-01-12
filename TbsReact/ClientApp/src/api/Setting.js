import axios from "axios";

const getChromeSetting = async (index) => {
	try {
		const { data } = await axios.get(`/accounts/${index}/settings/chrome`);
		return data;
	} catch (e) {
		console.log(e);
	}
};

export { getChromeSetting };
