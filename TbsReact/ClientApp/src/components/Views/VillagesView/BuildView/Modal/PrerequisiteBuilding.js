import { Grid, Button, Typography } from "@mui/material";
import React, { useState, useEffect, useContext } from "react";
import { useForm } from "react-hook-form";
import { useSelector } from "react-redux";

import ContentBox from "../../../../ContentBox";
import MUISelect from "../../../../ref/MUISelect";

import { getBuilds, addToQueue, PREREQUISITE } from "../../../../../api/api";
import { useVillage } from "../../../../../hooks/useVillage";

import { BuildingContext } from "../Build";
const PrerequisiteBuilding = () => {
	const { control, handleSubmit, setValue } = useForm();

	const account = useSelector((state) => state.account.info);
	const [buildingId] = useContext(BuildingContext);
	const [villageId] = useVillage();

	const [builds, setBuilds] = useState([
		{ id: 1, name: "Loading ..." },
		{ id: 2, name: "Loading ..." },
		{ id: 3, name: "Loading ..." },
	]);
	useEffect(() => {
		if (account.id !== -1 && villageId !== -1) {
			getBuilds(account.id, villageId, PREREQUISITE).then((data) => {
				setBuilds(data);
				if (data.length > 0) {
					setValue("building", data[0].id);
				}
			});
		}
	}, [account.id, villageId, buildingId, setValue]);

	const onSubmit = async (data) => {
		await addToQueue(account.id, villageId, PREREQUISITE, {
			building: builds[data.building].name,
		});
	};

	return (
		<>
			<ContentBox>
				<Typography variant="h5">Prerequisite Building</Typography>
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

export default PrerequisiteBuilding;
