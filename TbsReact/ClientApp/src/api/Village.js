import axios from "axios";

const getVillages = async (index) => {
	try {
		const { data } = await axios.get(`/villages/${index}`);
		return data;
	} catch (e) {
		console.log(e);
	}
};

const getVillageInfo = async (index) => {
	try {
		const { data } = await axios.get(`/village/${index}`);
		return data;
	} catch (e) {
		console.log(e);
	}
};

export { getVillages, getVillageInfo };
