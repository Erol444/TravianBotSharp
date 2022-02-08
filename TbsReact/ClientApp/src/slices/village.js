import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";

import { getVillageInfo } from "../api/Village";
export const fetchVillageByID = createAsyncThunk(
	"village/fetcInfohById",
	async (id, thunkAPI) => {
		const data = await getVillageInfo(id);
		return data;
	}
);

const initialState = {
	info: {
		id: -1,
		name: 1,
		coords: {
			x: 1,
			y: 1,
		},
	},
};

export const villageSlice = createSlice({
	name: "village",
	initialState,
	reducers: {
		setVillage: (state, action) => {
			state.info = action.payload;
		},

		resetVillage: (state, action) => {
			return initialState;
		},
	},
	extraReducers: (builder) => {
		builder.addCase(fetchVillageByID.fulfilled, (state, action) => {
			state.info = action.payload;
		});
	},
});

export const { setVillage, resetVillage } = villageSlice.actions;
export default villageSlice.reducer;
