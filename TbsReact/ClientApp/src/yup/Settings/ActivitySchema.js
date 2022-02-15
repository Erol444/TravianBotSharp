import * as yup from "yup";

const ActivitySchema = yup
	.object()
	.shape({
		work: yup.object().shape({
			min: yup
				.number()
				.min(1)
				.integer()
				.required("Minimum is required")
				.typeError("Minimum must be a number"),
			max: yup
				.number()
				.moreThan(
					yup.ref("min"),
					`Maximum must be greater than minimum`
				)
				.integer()
				.required("Maximum is required")
				.typeError("Maximum must be a number"),
		}),
		sleep: yup.object().shape({
			min: yup
				.number()
				.min(1)
				.integer()
				.required("Minimum is required")
				.typeError("Minimum must be a number"),
			max: yup
				.number()
				.moreThan(
					yup.ref("min"),
					`Maximum must be greater than minimum`
				)
				.integer()
				.required("Maximum is required")
				.typeError("Maximum must be a number"),
		}),
	})
	.required();

export default ActivitySchema;
