import axios from "axios";

const getLogData = async (index) => {
	try {
		const { data } = await axios.get(`/logger/${index}`);
		return data;
	} catch (e) {
		console.log(e);
	}
};

const getTaskList = async (index) => {
	try {
		const { data } = await axios.get(`/task/${index}`);
		return data;
	} catch (e) {
		if (e.response.status === 404) {
			return null;
		}
		console.log(e);
	}
};

export { getLogData, getTaskList };
