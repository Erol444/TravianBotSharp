import axios from "axios";

const getBuildingList = async (accountIndex, villageIndex) => {
	try {
		const { data } = await axios.get(
			`/villages/${accountIndex}/build/${villageIndex}/buildings`
		);
		return data;
	} catch (e) {
		console.log(e);
	}
};

const getCurrentList = async (accountIndex, villageIndex) => {
	try {
		const { data } = await axios.get(
			`/villages/${accountIndex}/build/${villageIndex}/current`
		);
		return data;
	} catch (e) {
		console.log(e);
	}
};

const getQueueList = async (accountIndex, villageIndex) => {
	try {
		const { data } = await axios.get(
			`/villages/${accountIndex}/build/${villageIndex}/queue`
		);
		return data;
	} catch (e) {
		console.log(e);
	}
};

const addToQueue = async (accountIndex, villageIndex, type, building) => {
	try {
		const { data } = await axios.post(
			`/villages/${accountIndex}/build/${villageIndex}/queue/${type}`,
			building
		);
		return data;
	} catch (e) {
		console.log(e);
	}
};

const editQueue = async (accountIndex, villageIndex, position) => {
	try {
		const { data } = await axios.patch(
			`/villages/${accountIndex}/build/${villageIndex}/queue/`,
			position
		);
		return data;
	} catch (e) {
		console.log(e);
	}
};

const deleteQueue = async (accountIndex, villageIndex, position) => {
	try {
		const { data } = await axios.delete(
			`/villages/${accountIndex}/build/${villageIndex}/queue/${position}`
		);
		return data;
	} catch (e) {
		console.log(e);
	}
};

const getBuilds = async (accountIndex, villageIndex, type, position) => {
	try {
		const { data } = await axios.get(
			`/villages/${accountIndex}/build/${villageIndex}/buildings/${type}/${
				position ?? ""
			}`
		);
		return data;
	} catch (e) {
		console.log(e);
	}
};
export {
	getBuildingList,
	getCurrentList,
	getQueueList,
	addToQueue,
	editQueue,
	deleteQueue,
	getBuilds,
};
