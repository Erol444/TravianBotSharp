import axios from "axios";

const login = async (index) => {
	try {
		await axios.post(`/accounts/login/${index}`);
		return true;
	} catch (e) {
		console.log(e);
		return false;
	}
};

const logout = async (index) => {
	try {
		await axios.post(`/accounts/logout/${index}`);
		return true;
	} catch (e) {
		console.log(e);
		return true;
	}
};

const getStatus = async (index) => {
	try {
		const { data } = await axios.get(`/status/${index}`);
		return data;
	} catch (e) {
		console.log(e);
		return false;
	}
};

export { login, logout, getStatus };
