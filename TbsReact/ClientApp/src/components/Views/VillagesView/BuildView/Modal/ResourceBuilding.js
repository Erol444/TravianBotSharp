import { Grid, TextField, Button, Typography } from "@mui/material";
import React from "react";
import { useForm } from "react-hook-form";

import ContentBox from "../../../../ContentBox";
import MUISelect from "../../../../ref/MUISelect";

const ResourceBuilding = () => {
	const {
		register,
		control,
		handleSubmit,
		formState: { errors },
	} = useForm();

	const onSubmit = (data) => {
		console.log(data);
	};

	const type = [
		{ id: 1, name: "All resource" },
		{ id: 2, name: "Exclude crop" },
		{ id: 3, name: "Only crop" },
	];
	const strategy = [
		{ id: 1, name: "Based on resource" },
		{ id: 2, name: "Based on level" },
		{ id: 3, name: "Based on production" },
	];
	return (
		<>
			<ContentBox>
				<Typography variant="h5">Resource auto build</Typography>
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
