import axios from "axios";

const getAccesses = async (index) => {
	try {
		const { data } = await axios.get(`/accounts/${index}/accesses`);
		return data;
	} catch (e) {
		console.log(e);
	}
};

export { getAccesses };
