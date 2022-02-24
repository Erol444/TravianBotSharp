import { Grid, TextField, Button, Typography } from "@mui/material";
import React from "react";
import { useForm } from "react-hook-form";
import { useSelector } from "react-redux";

import ContentBox from "../../../../ContentBox";
import MUISelect from "../../../../ref/MUISelect";

import { addToQueue, RESOURCE } from "../../../../../api/api";
import { useVillage } from "../../../../../hooks/useVillage";

const ResourceBuilding = () => {
	const {
		register,
		control,
		handleSubmit,
		formState: { errors },
	} = useForm({
		defaultValues: {
			type: 0,
			strategy: 0,
			level: 1,
		},
	});

	const account = useSelector((state) => state.account.info);
	const [villageId] = useVillage();

	const onSubmit = async (data) => {
		await addToQueue(account.id, villageId, RESOURCE, data);
	};

	const type = [
		{ id: 0, name: "All resource" },
		{ id: 1, name: "Exclude crop" },
		{ id: 2, name: "Only crop" },
	];
	const strategy = [
		{ id: 0, name: "Based on resource" },
		{ id: 1, name: "Based on level" },
		{ id: 2, name: "Based on production" },
	];

	return (
		<>
			<ContentBox>
				<Typography variant="h5">Resource auto build</Typography>
				<br />
				<form onSubmit={handleSubmit(onSubmit)}>
					<Grid container spacing={3}>
						<Grid item xs={12}>
							<MUISelect
								label="Resource type"
								name="type"
								control={control}
								options={type}
								fullWidth
							/>
						</Grid>
						<Grid item xs={12}>
							<MUISelect
								label="Build strategy"
								name="strategy"
								control={control}
								options={strategy}
								fullWidth
							/>
						</Grid>
						<Grid item xs={12}>
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

export default ResourceBuilding;
