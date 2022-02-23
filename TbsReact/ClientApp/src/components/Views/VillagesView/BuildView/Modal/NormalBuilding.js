import { Grid, TextField, Button, Typography } from "@mui/material";
import React, { useState, useEffect, useContext } from "react";
import { useForm } from "react-hook-form";
import { useSelector } from "react-redux";

import ContentBox from "../../../../ContentBox";
import MUISelect from "../../../../ref/MUISelect";

import { getBuilds, addToQueue, NORMAL } from "../../../../../api/api";
import { useVillage } from "../../../../../hooks/useVillage";

import { BuildingContext } from "../Build";

const NormalBuilding = () => {
	const {
		register,
		control,
		handleSubmit,
		formState: { errors },
		setValue,
	} = useForm();
	const account = useSelector((state) => state.account.info);
	const [buildingId] = useContext(BuildingContext);
	const [villageId] = useVillage();

	const [builds, setBuilds] = useState([
		{ id: 1, name: "Loading ..." },
		{ id: 2, name: "Loading ..." },
		{ id: 3, name: "Loading ..." },
	]);

	useEffect(() => {
		if (account.id !== -1 && villageId !== -1 && buildingId !== -1) {
			getBuilds(account.id, villageId, NORMAL, buildingId).then(
				(data) => {
					const { buildList, level } = data;
					setBuilds(buildList);
					setValue("level", level);
					if (buildList.length > 0) {
						setValue("building", buildList[0].id);
					}
				}
			);
		}
	}, [account.id, villageId, buildingId, setValue]);
	const onSubmit = async (data) => {
		const request = {
			building: builds[data.building].name,
			level: data.level,
			location: buildingId + 1,
		};
		await addToQueue(account.id, villageId, NORMAL, request);
	};
	return (
		<>
			<ContentBox>
				<Typography variant="h5">Normal Building</Typography>
				<br />

				<form onSubmit={handleSubmit(onSubmit)}>
					<Grid container spacing={3}>
						<Grid item xs={8}>
							<MUISelect
								label="Building"
								name="building"
								control={control}
								options={builds}
								fullWidth
							/>
						</Grid>
						<Grid item xs={4}>
							<TextField
								fullWidth
								label="Level"
								variant="outlined"
								type="number"
								error={!!errors.level}
								helperText={errors.level?.message}
								{...register("level")}
							/>
						</Grid>
						<Grid item xs={12}>
							<Button
								type="submit"
								variant="contained"
								value="submit"
								color="success"
							>
								Build
							</Button>
						</Grid>
					</Grid>
				</form>
			</ContentBox>
		</>
	);
};

export default NormalBuilding;
