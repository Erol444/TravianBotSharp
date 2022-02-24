import axios from "axios";

const getSetting = async (index, type) => {
	try {
		const { data } = await axios.get(`/settings/${type}/${index}`);
		return data;
	} catch (e) {
		return e.response.status;
	}
};

const setSetting = async (index, type, data) => {
	try {
		await axios.put(`/settings/${type}/${index}/`, data);
		return true;
	} catch (e) {
		return e.response.status;
	}
};

export { getSetting, setSetting };
