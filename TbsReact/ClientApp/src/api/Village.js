import axios from "axios";

const getVillages = async (index) => {
	try {
		const { data } = await axios.get(`/villages/${index}`);
		return data;
	} catch (e) {
		console.log(e);
	}
};

export { getVillages };
